using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EarthquakeReporter.Services;
using Microsoft.Extensions.Logging;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _fromEmail;
    private readonly string _smtpPassword;
    private readonly ILogger<EmailService> _logger;

    public EmailService(string smtpServer, int smtpPort, string fromEmail, string smtpPassword, ILogger<EmailService> logger)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _fromEmail = fromEmail;
        _smtpPassword = smtpPassword;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            _logger.LogInformation($"Sending email to {toEmail} with subject '{subject}'");

            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_fromEmail, _smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation("Email sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email.");
        }
    }
}
