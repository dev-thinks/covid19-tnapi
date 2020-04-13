using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Service.Infrastructure.Models;
using Service.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Infrastructure.Middlewares
{
    /// <summary>
    /// Extends <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> with methods for exception handling
    /// </summary>
    public static class ErrorHandlingExtensions
    {
        /// <summary>
        /// Adds middleware for streamlined exception handling. This will sends an email to configured users with the exception message
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseServiceErrorLogging(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }

    /// <summary>
    /// Middleware to capture all exceptions from API
    /// </summary>
    internal class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEmailHelper _emailHelper;
        private readonly ExceptionEmailOptions _options;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        /// <param name="emailHelper"></param>
        public ErrorHandlingMiddleware(RequestDelegate next, IOptions<ExceptionEmailOptions> options, IEmailHelper emailHelper)
        {
            _next = next;
            _emailHelper = emailHelper;
            _options = options.Value;
        }

        /// <summary>
        /// Capturing the exception, returning gist message with request id.
        /// The detailed exception will be searched by this request id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Pipeline exception is handled here.
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // 500 if unexpected
            var code = context.Response.StatusCode;

            // do not override status code and content type when it is already set
            if (code < 400)
            {
                code = 500;

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = code;

                // https://github.com/aspnet/AspNetCore/issues/9491#issuecomment-488764101
                var traceId = Activity.Current.RootId;

                // getting the inner exception message for detail message to client, otherwise exception message
                var errorMessage = ex.InnerException?.Message ?? ex.Message;

                // Serialize return error response with context request id, trace id, error message and status code
                // Serialized with System.Text.Json library.
                var result = JsonSerializer.Serialize(new
                {
                    RequestId = context.TraceIdentifier,
                    TraceId = traceId,
                    Message = errorMessage,
                    StatusCode = code
                }, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });

                var division = context.Request.Headers
                    .Where(s => string.Equals(s.Key, "businessunit", StringComparison.OrdinalIgnoreCase))
                    .Select(h => h.Value.ToString())
                    .FirstOrDefault()?.ToUpper();

                //ThreadingService.FireAndForget(() => NotifyException(ex, division, result));

                // setting the error message to response with code and content type to the client.
                return context.Response.WriteAsync(result);
            }

            return Task.CompletedTask;
        }

        private void NotifyException(Exception ex, string division, string additionalInfo)
        {
            var prefix = $"Service Connect MicroService: {_options.MicroServiceName}\n" +
                         $"\n{additionalInfo}\n";

            var errorInfo = $"{prefix}\n{ex}";

            var emailRequest = new EmailMessageModel
            {
                IsHtml = false,
                EmailSubject = _options.EmailSubject,
                EmailBody = errorInfo,
                FromEmail = new EmailAddressModel {Address = _options.FromAddress, DisplayName = $"{division} Web Services"},
                ToAddresses = GetEmailTo(_options.EmailTo)
            };

            _ = _emailHelper.SendEmail(emailRequest);
        }

        private List<EmailAddressModel> GetEmailTo(string emailTos)
        {
            var addresses = emailTos.Split(",;".ToCharArray()).ToList();

            return addresses.Select(address => new EmailAddressModel {Address = address, DisplayName = address})
                .ToList();
        }
    }
}
