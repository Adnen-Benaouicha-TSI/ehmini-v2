using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Ehmini.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Ehmini.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(string receiver, string subject, string message)
    {
        try
        {
            var senderAdr = _configuration["EmailSettings:Email"];
            var senderPwd = _configuration["EmailSettings:Password"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "ssl0.ovh.net";
            var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");

            using SmtpClient client = new SmtpClient(smtpServer, port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderAdr, senderPwd),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderAdr!);
            mail.To.Add(receiver);
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;

            await client.SendMailAsync(mail);
            return true;
        }
        catch (Exception ex)
        {
            // Remplace par ton système de log (ex: Serilog ou ILogger) si nécessaire
            Console.WriteLine($"Erreur SendEmail : {ex.Message}");
            return false;
        }
    }
}