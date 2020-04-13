using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Service.Infrastructure.Models;

namespace Service.Infrastructure.Middlewares
{
    /// <summary>
    /// Extends <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> with methods for configuring additional logging.
    /// </summary>
    public static class RequestLoggingExtensions
    {
        /// <summary>
        /// Adds middleware for streamlined request logging. Instead of writing HTTP request information
        /// like method, path, timing, status code and exception details
        /// in several events, this middleware collects information during the request (including from
        /// <see cref="T:Serilog.IDiagnosticContext" />), and writes a single event at request completion.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseServiceRequestLogging(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ServiceRequestLoggingMiddleware>();
        }
    }

    internal class ServiceRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MessageTemplate _messageTemplate;

        public ServiceRequestLoggingMiddleware(RequestDelegate next, IOptions<ExceptionEmailOptions> options)
        {
            var appOptions = options.Value;
            RequestDelegate requestDelegate = next;

            var strMessageFormat =
                $"[{appOptions.MicroServiceName}] HTTP {{RequestMethod}} {{RequestPath}} responded {{StatusCode}} " +
                $"in {{Elapsed:0.0000}} ms with TraceId: {{traceId}}, RequestId: {{requestId}}";

            _next = requestDelegate;
            _messageTemplate = new MessageTemplateParser().Parse(strMessageFormat);
        }

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            // get the incoming request
            var request = await FormatRequest(httpContext.Request);

            // Copy a pointer to the original response body stream
            var originalBodyStream = httpContext.Response.Body;

            var start = Stopwatch.GetTimestamp();

            try
            {
                // Create a new memory stream
                using (var responseBody = new MemoryStream())
                {
                    var response = httpContext.Response;
                    response.Body = responseBody;

                    // attaching current trace id into the response header
                    httpContext.Response.OnStarting(() =>
                    {
                        httpContext.Response.Headers["x-service-traceid"] = Activity.Current.RootId;
                        return Task.CompletedTask;
                    });

                    // Continue down the Middleware pipeline, eventually returning to this class
                    await _next(httpContext);

                    var elapsedMilliseconds = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());
                    var statusCode = httpContext.Response.StatusCode;

                    // Format the response from the server
                    var responseBodyContent = await FormatResponse(httpContext.Response);

                    // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                    await responseBody.CopyToAsync(originalBodyStream);

                    // logs the message
                    LogCompletion(httpContext, statusCode, elapsedMilliseconds, null, request, responseBodyContent);
                }
            }
            catch (Exception ex) when (LogCompletion(httpContext, 500, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), ex, request))
            {
                // when condition method always returning 'false', so exception is NOT handled here.

                // exception will be handled by "ErrorHandlingMiddleware" (next in pipeline).
            }
            finally
            {
                // restoring the actual response stream with new response to the context,
                // otherwise ends up with 'Cannot access a closed Stream' exception
                httpContext.Response.Body = originalBodyStream;
            }
        }

        private bool LogCompletion(HttpContext httpContext, int statusCode, double elapsedMs, Exception ex, string request = "", string response = "")
        {
            ILogger logger = Log.ForContext<ServiceRequestLoggingMiddleware>();

            // When status code is >= 400 or exception is not null, log as Error.
            LogEventLevel level = statusCode >= 400 || ex != null ? LogEventLevel.Error : LogEventLevel.Debug;

            if (!logger.IsEnabled(level))
            {
                return false;
            }

            // truncated logs for 1000 chars in request/response body
            if (request.Length > 1000)
            {
                request = $"(Truncated to first 1000 chars)|{request.Substring(0, 1000)}";
            }

            if (response.Length > 1000)
            {
                response = $"(Truncated to first 1000 chars)|{response.Substring(0, 1000)}";
            }

            // https://github.com/aspnet/AspNetCore/issues/9491#issuecomment-488764101
            var traceId = Activity.Current.RootId;
            var spanId = Activity.Current.Id;
            var parentId = Activity.Current.ParentId;

            var requestHeaders = httpContext.Request.Headers
                .Where(s => !string.IsNullOrEmpty(s.Key))
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            logger = logger
                .ForContext("RequestBody", request)
                .ForContext("ResponseBody", response)
                .ForContext("TraceId", traceId)
                .ForContext("SpanId", spanId)
                .ForContext("ParentId", parentId)
                .ForContext("RequestHeaders", requestHeaders);

            IEnumerable<LogEventProperty> properties =
                new[]
                {
                    new LogEventProperty("RequestMethod", new ScalarValue(httpContext.Request.Method)),
                    new LogEventProperty("RequestPath", new ScalarValue(GetPath(httpContext))),
                    new LogEventProperty("StatusCode", new ScalarValue(statusCode)),
                    new LogEventProperty("Elapsed", new ScalarValue(elapsedMs)),
                    new LogEventProperty("traceId", new ScalarValue(traceId)),
                    new LogEventProperty("requestId", new ScalarValue(httpContext.TraceIdentifier))
                };

            LogEvent logEvent = new LogEvent(DateTimeOffset.Now, level, ex, _messageTemplate, properties);
            logger.Write(logEvent);

            return false;
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            //var body = request.Body;

            // This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            // We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            // Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            // We convert the byte[] into a string using UTF8 encoding.
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            // assign the read body back to the request body, which is allowed because of EnableBuffering()
            //request.Body = body;
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            // We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            // and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            // We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            // Return the string for the response
            return text;
        }

        private static double GetElapsedMilliseconds(long start, long stop)
        {
            return (double)((stop - start) * 1000L) / Stopwatch.Frequency;
        }

        private static string GetPath(HttpContext httpContext)
        {
            return httpContext.Features.Get<IHttpRequestFeature>()?.RawTarget ?? httpContext.Request.Path.ToString();
        }
    }
}
