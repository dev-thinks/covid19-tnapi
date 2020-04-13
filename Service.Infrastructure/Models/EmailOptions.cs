using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace Service.Infrastructure.Models
{
    /// <summary>
    /// Email message model
    /// </summary>
    public class EmailMessageModel
    {
        /// <summary>
        /// Email address
        /// </summary>
        public EmailAddressModel FromEmail { get; set; }

        /// <summary>
        /// Email To Address
        /// </summary>
        public List<EmailAddressModel> ToAddresses { get; set; }

        /// <summary>
        /// Email To carbon copy Address
        /// </summary>
        public List<EmailAddressModel> CcAddresses { get; set; }

        /// <summary>
        /// Email To blind carbon copy Address
        /// </summary>
        public List<EmailAddressModel> BccToAddresses { get; set; }

        /// <summary>
        /// Email body
        /// </summary>
        public string EmailBody { get; set; }

        /// <summary>
        /// Email subject
        /// </summary>
        public string EmailSubject { get; set; }

        /// <summary>
        /// Is email content html or text
        /// </summary>
        public bool IsHtml { get; set; }

        /// <summary>
        /// Reply To email
        /// </summary>
        public string ReplyToEmail { get; set; }

        public List<EmailMessageAttachmentModel> Attachments { get; set; }

        /// <summary>
        /// Converts this Notification to a <see cref="System.Net.Mail.MailMessage">MailMessage</see>.
        /// </summary>
        /// <returns><see cref="System.Net.Mail.MailMessage">MailMessage</see></returns>
        public MailMessage ToMailMessage()
        {
            MailMessage message = new MailMessage { Subject = EmailSubject, Body = EmailBody, IsBodyHtml = IsHtml };

            if (!string.IsNullOrEmpty(ReplyToEmail))
            {
                MailAddress replyToEmail = new MailAddress(ReplyToEmail, ReplyToEmail);
                message.ReplyToList.Add(replyToEmail);
            }

            if (FromEmail != null)
            {
                message.From = FromEmail.ToMailAddress();
            }

            foreach (var address in ToAddresses)
            {
                message.To.Add(address.ToMailAddress());
            }

            if (CcAddresses != null && CcAddresses.Count > 0)
            {
                foreach (var address in CcAddresses)
                {
                    message.CC.Add(address.ToMailAddress());
                }
            }

            if (BccToAddresses != null && BccToAddresses.Count > 0)
            {
                foreach (var address in BccToAddresses)
                {
                    message.Bcc.Add(address.ToMailAddress());
                }
            }

            if (Attachments != null && Attachments.Count > 0)
            {
                foreach (var attachment in Attachments)
                {
                    message.Attachments.Add(attachment.ToMailAttachment());
                }
            }

            return message;
        }
    }

    /// <summary>
    /// Email message attachment model
    /// </summary>
    public class EmailMessageAttachmentModel
    {
        /// <summary>
        /// File Name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// File stream bytes
        /// </summary>
        public byte[] FileBytes { get; set; }

        /// <summary>
        /// Returns this EmailAttachment as a Mail Attachment.
        /// </summary>
        /// <returns>Attachment</returns>
        public Attachment ToMailAttachment()
        {
            Stream data = new MemoryStream(FileBytes);
            Attachment attach = new Attachment(data, FileName);

            return attach;
        }
    }

    /// <summary>
    /// Email address model
    /// </summary>
    public class EmailAddressModel
    {
        /// <summary>
        /// Email address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Email address display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Converts this EmailAddress to a <see cref="System.Net.Mail.MailAddress">System.Net.Mail.MailAddress</see>
        /// </summary>
        /// <returns><see cref="System.Net.Mail.MailAddress">MailAddress</see></returns>
        public MailAddress ToMailAddress()
        {
            return new MailAddress(Address, DisplayName);
        }
    }
}
