using System;
using System.Net;
using System.Net.Mail;
using JwtAuthProject.Application.Common;
using JwtAuthProject.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace JwtAuthProject.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IOptions<EmailSettings> _options;

    public EmailService(IOptions<EmailSettings> options)
    {
        _options = options;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            using var smtpClient = new SmtpClient(_options.Value.SmtpServer, _options.Value.Port);
            smtpClient.Credentials = new NetworkCredential(_options.Value.Email, _options.Value.Password);
            smtpClient.EnableSsl = true;

            var mail = new MailMessage(_options.Value.Email, to, subject, body);
            mail.IsBodyHtml = true;

            await smtpClient.SendMailAsync(mail);
            return true;
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine($"Error {e.Message}");
            return false;
        }
    }
}
