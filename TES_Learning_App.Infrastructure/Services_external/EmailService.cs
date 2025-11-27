using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.Common;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.Infrastructure.Services_external
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var apiKey = _emailSettings.SendGridApiKey;
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("SendGrid API Key is not configured.");
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                //_logger.LogError($"Failed to send email: {await response.Body.ReadAsStringAsync()}");
            }
        }
    }
}
