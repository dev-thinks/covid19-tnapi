using Serilog;
using Service.Infrastructure.Models;
using System;
using System.Net.Mail;

namespace Service.Infrastructure.Utils
{
    public interface IEmailHelper
    {
        bool SendEmail(EmailMessageModel message);
    }

    /// <summary>
    /// Helper for sending emails
    /// </summary>
    public class EmailHelper : IEmailHelper
    {
        private readonly string _host;

        public EmailHelper(string host)
        {
            _host = host;
        }

        /// <summary>
        /// Sending email with Mail message and attachment
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendEmail(EmailMessageModel message)
        {
            SmtpClient client = new SmtpClient(_host);
            ILogger logger = Log.ForContext<EmailHelper>();

            try
            {
                client.Send(message.ToMailMessage());
            }
            catch (SmtpException e)
            {
                logger.Error(e, "SMTPException while sending email.");

                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while sending email.");

                return false;
            }
            finally
            {
                if (client != null)
                {
                    client = null;
                }
            }

            return true;
        }
    }
}
