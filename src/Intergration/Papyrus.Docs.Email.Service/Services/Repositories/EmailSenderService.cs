using MailKit.Net.Smtp;
using MimeKit;
using Papyrus.Docs.Email.Service.Configurations;
using Papyrus.Docs.Email.Service.Models;
using Papyrus.Docs.Email.Service.Services.Interfaces;
using Papyrus.Docs.Email.Service.Templates;

namespace Papyrus.Docs.Email.Service.Services.Repositories
{
    /// <summary>
    /// Email sender service
    /// </summary>
    /// <param name="emailConfiguration"> Email configuration to use for sending emails </param>
    /// <param name="razorRenderer"> Razor view to string renderer </param>
    public class EmailSenderService(EmailConfiguration emailConfiguration, RazorViewToStringRenderer razorRenderer) : IEmailSenderService
    {
        #region Private methods
        /// <summary>
        /// Create a new email message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<MimeMessage> CreateMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Admin", emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var emailTemplate = new EmailTemplateModel
            {
                Subject = message.Subject,
                Content = message.Content
            };

            string body = await razorRenderer.RenderViewToStringAsync("EmailTemplate", emailTemplate);

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = body
            };

            return emailMessage;
        }

        /// <summary>
        /// Send the email message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool Send(MimeMessage message)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(emailConfiguration.SmtpServer, emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(emailConfiguration.UserName, emailConfiguration.Password);
                client.Send(message);

                return true;
            }
            catch (Exception)
            {
                // add logging here later
                return false;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
        #endregion

        public async Task<bool> SendEmailAsync(Message message)
        {
            var emailMessage = await CreateMessage(message);

            return Send(emailMessage);
        }
    }
}
