namespace Service.Infrastructure.Models
{
    /// <summary>
    /// Options for sending email when exception
    /// </summary>
    public class ExceptionEmailOptions
    {
        public string FromAddress { get; set; }

        public string EmailSubject { get; set; }

        public string EmailTo { get; set; }

        public string MicroServiceName { get; set; }
    }
}
