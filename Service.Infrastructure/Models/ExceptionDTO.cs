namespace Service.Common.Model
{
    /// <summary>
    /// Exception response DTO model
    /// </summary>
    public class ExceptionResponseDTO
    {
        /// <summary>
        /// Error Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Exception message
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Exception type
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Exception stack trace
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Inner exception
        /// </summary>
        public ExceptionResponseDTO InnerException { get; set; }
    }
}
