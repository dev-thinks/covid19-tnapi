namespace Service.Infrastructure.Models
{
    public class AppConfigurationOptions
    {
        public string MicroServiceName { get; set; }

        public string DefaultConnection { get; set; }

        public string Redis { get; set; }

        public string SwaggerVersion { get; set; }

        public string SwaggerTitle { get; set; }

        public string SwaggerDescription { get; set; }

        public string SwaggerTeamName { get; set; }

        public string SwaggerTeamContact { get; set; }

        public string SwaggerTermsOfService { get; set; }

        public string XmlCommentsFilePath { get; set; }

        public string SplunkHost { get; set; }

        public string SplunkToken { get; set; }

        public string SplunkURI { get; set; }

        public string SplunkSource { get; set; }

        public string SplunkSourceType { get; set; }

        public string InstallEnvironment { get; set; }

        public string ErrorTo { get; set; }

        public string ErrorFrom { get; set; }

        public string Subject { get; set; }

        public string SmtpHost { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SecretKey { get; set; }

        public int JwtTimeout { get; set; }

        public int CacheTimeout { get; set; }
    }
}
