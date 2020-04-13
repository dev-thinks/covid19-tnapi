using Refit;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Service.Common.Utils
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const long OneTb = OneGb * 1024;

        /// <summary>
        /// Converts integer bytes into pretty memory size in KB/MB/GB
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static string ToPrettyMemorySize(this int value, int decimalPlaces = 0)
        {
            return ((long) value).ToPrettyMemorySize(decimalPlaces);
        }

        /// <summary>
        /// Converts integer bytes into pretty memory size in KB/MB/GB
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static string ToPrettyMemorySize(this long value, int decimalPlaces = 0)
        {
            var asTb = Math.Round((double) value / OneTb, decimalPlaces);
            var asGb = Math.Round((double) value / OneGb, decimalPlaces);
            var asMb = Math.Round((double) value / OneMb, decimalPlaces);
            var asKb = Math.Round((double) value / OneKb, decimalPlaces);
            string chosenValue = asTb > 1 ? $"{asTb} TB"
                : asGb > 1 ? $"{asGb} GB"
                : asMb > 1 ? $"{asMb} MB"
                : asKb > 1 ? $"{asKb} KB"
                : $"{Math.Round((double) value, decimalPlaces)} B";
            return chosenValue;
        }

        /// <summary>
        /// Converts integer bytes into pretty memory size in MB
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static string ToPrettyMemorySizeInMb(this long value, int decimalPlaces = 0)
        {
            var asMb = Math.Round((double) value / OneMb, decimalPlaces);
            string chosenValue = $"{asMb:0.00} MB";
            return chosenValue;
        }

        /// <summary>
        /// Sorts the list of Date time values in Descending
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<DateTime> SortDescending(this List<DateTime> list)
        {
            list.Sort((a, b) => b.CompareTo(a));
            return list;
        }

        /// <summary>
        /// Gets the description property of the enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    // Pattern matching
                    if (System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute
                        attr)
                    {
                        return attr.Description;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns list of data by filtering distinct by a column
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }


        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        /// <summary>
        /// Gets client Ip address
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets request server variable
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetServerVariable(this HttpRequestMessage request, string name)
        {
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.ServerVariables[name];
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified list contains the matching string value
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="value">The value to match.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the case is ignored.</param>
        /// <returns>
        ///   <c>true</c> if the specified list contains the matching string; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this List<string> list, string value, bool ignoreCase = false)
        {
            return ignoreCase ?
                list.Any(s => s.Equals(value, StringComparison.OrdinalIgnoreCase)) :
                list.Contains(value);
        }

        /// <summary>
        /// Difference between 2 dates in months
        /// </summary>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        /// <returns></returns>
        public static double DifferenceInMonths(this DateTime from, DateTime to)
        {
            //Compute full months difference between dates
            var fullMonthsDiff = (to.Year - from.Year) * 12 + to.Month - from.Month;

            //Compute difference between the % of day to full days of each month
            var fractionMonthsDiff = ((double) (to.Day - 1) / (DateTime.DaysInMonth(to.Year, to.Month) - 1)) -
                                     ((double) (from.Day - 1) / (DateTime.DaysInMonth(from.Year, from.Month) - 1));

            return fullMonthsDiff + fractionMonthsDiff;
        }

        /// <summary>
        /// Returns the base64 equivalent of the plaintext string
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Unwraps exception information
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ToPrettyString(this Exception ex)
        {
            try
            {
                var errorInfo = new StringBuilder(string.Empty);

                switch (ex)
                {
                    case AggregateException agEx:
                        {
                            foreach (var exception in agEx.Flatten().InnerExceptions)
                            {
                                if (exception is ApiException agRefitException)
                                {
                                    try
                                    {
                                        errorInfo.AppendLine(agRefitException.ToString());
                                        errorInfo.AppendLine($"API Request URL: {agRefitException.Uri}").AppendLine();
                                        errorInfo.AppendLine($"API Request Message: {agRefitException.RequestMessage.SerializeAndFormat()}");
                                        errorInfo.AppendLine($"API Error Content: {agRefitException.Content.SerializeAndFormat()}");
                                    }
                                    catch (Exception)
                                    {
                                        errorInfo.AppendLine(agRefitException.Content);
                                    }
                                }
                                else
                                {
                                    errorInfo.AppendLine(exception.ToString());
                                }
                            }

                            break;
                        }
                    case ApiException refitEx:
                        {
                            try
                            {
                                errorInfo.AppendLine(refitEx.ToString());
                                errorInfo.AppendLine($"API Request URL: {refitEx.Uri}").AppendLine();
                                errorInfo.AppendLine($"API Request Message: {refitEx.RequestMessage.SerializeAndFormat()}");
                                errorInfo.AppendLine($"API Error Content: {refitEx.Content.SerializeAndFormat()}");

                                //var refitContent = refitEx.GetContentAsAsync<Dictionary<string, string>>().Result;

                                //errorInfo.Append(string.Join(";", refitContent.Select(x => x.Key + "=" + x.Value)));
                            }
                            catch (Exception)
                            {
                                errorInfo.AppendLine(refitEx.Content);
                            }

                            break;
                        }
                    default:
                        {
                            errorInfo.AppendLine(ex.ToString());

                            break;
                        }
                }

                return errorInfo.ToString();
            }
            catch
            {
                // NB: We're already handling an exception at this point, 
                // so we want to make sure we don't throw another one 
                // that hides the real error.

                return ex.ToString();
            }
        }
    }

}
