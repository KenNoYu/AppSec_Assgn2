using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridClient _sendGridClient;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger)
        {
            var apiKey = configuration["SendGrid:ApiKey"];
            _sendGridClient = new SendGridClient(apiKey);
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var client = _sendGridClient;
                var from = new EmailAddress("tanjy0206@gmail.com", "Ace Job Agency");
                var to = new EmailAddress(email);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
                var response = await _sendGridClient.SendEmailAsync(msg);

                _logger.LogInformation($"Email sent to {email}. Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Email sent to {email}. Status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
            }
        }
    }
}
