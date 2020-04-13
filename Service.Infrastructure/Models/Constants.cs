// ReSharper disable InconsistentNaming
namespace Service.Infrastructure.Models
{
    /// <summary>
    /// Service connect Error codes
    /// </summary>
    public class ErrorCodes
    {
        /// <summary>
        /// The application has encountered an unknown error.
        /// </summary>
        public static readonly string DEFAULT = "The application has encountered an unknown error.";

        /// <summary>
        /// One or more model validation errors occurred while processing the request.
        /// </summary>
        public static readonly string MODEL_VALIDATION = "One or more model validation errors occurred while processing the request.";

        /// <summary>
        /// Invalid business unit/division id is supplied as part of header.
        /// </summary>
        public static readonly string CODE_501 = "Invalid business unit/division id is supplied as part of header.";

        /// <summary>
        /// Api request rejected due to unavailable business unit/division id.
        /// </summary>
        public static readonly string CODE_502 = "Api request rejected due to unavailable business unit/division id.";
    }
}
