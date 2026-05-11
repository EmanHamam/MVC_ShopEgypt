using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
namespace ShopEgypt.Infrastructure.ExternalServices.SendGridEmailSender
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SendGridEmailSender> _logger;

        public SendGridEmailSender(IConfiguration config, ILogger<SendGridEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var apiKey = _config["SendGrid:ApiKey"];
                var fromEmail = _config["SendGrid:FromEmail"];
                var fromName = _config["SendGrid:FromName"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("SendGrid API key is not configured.");
                    throw new InvalidOperationException("SendGrid:ApiKey is not configured in appsettings.json");
                }

                if (string.IsNullOrEmpty(fromEmail))
                {
                    _logger.LogError("SendGrid FromEmail is not configured.");
                    throw new InvalidOperationException("SendGrid:FromEmail is not configured in appsettings.json");
                }

                var client = new SendGridClient(apiKey);

                var from = new EmailAddress(fromEmail, fromName);
                var to = new EmailAddress(email);

                var msg = MailHelper.CreateSingleEmail(
                    from,
                    to,
                    subject,
                    plainTextContent: null,
                    htmlContent: htmlMessage
                );

                var response = await client.SendEmailAsync(msg);

                // Check if the email was sent successfully
                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Body.ReadAsStringAsync();

                    // Provide specific error messages
                    string errorDetail = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.Forbidden => $"403 Forbidden: The 'From Email' ({fromEmail}) is not verified in SendGrid. " +
                            "Please verify this email address in SendGrid Dashboard → Sender Authentication, or use a verified sender email.",
                        System.Net.HttpStatusCode.Unauthorized => "401 Unauthorized: Invalid API key. Check your SendGrid API key in appsettings.json.",
                        System.Net.HttpStatusCode.BadRequest => $"400 Bad Request: {responseBody}",
                        _ => $"Status {response.StatusCode}: {responseBody}"
                    };

                    _logger.LogError($"Failed to send email to {email}. {errorDetail}");
                    throw new InvalidOperationException($"Failed to send email. {errorDetail}");
                }

                _logger.LogInformation($"Email successfully sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {email}: {ex.Message}");
                throw;
            }
        }
    }
}
