using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Configurations;

namespace Rex.Infrastructure.Shared.Services;

public class EmailService(IOptions<EmailConfiguration> emailOptions): IEmailService
{
    private EmailConfiguration _emailConfiguration { get; } = emailOptions.Value;

    public async Task SendEmailAsync(EmailDto emailAnswer)
    {
        try
        {
            MimeMessage email = new();
            email.Sender = MailboxAddress.Parse(_emailConfiguration.EmailFrom);
            email.To.Add(MailboxAddress.Parse(emailAnswer.User));
            email.Subject = emailAnswer.Subject;
            BodyBuilder bodyBuilder = new()
            {
                HtmlBody = emailAnswer.Body,
            };
            email.Body = bodyBuilder.ToMessageBody();

            using SmtpClient smtp = new();
            smtp.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            await smtp.ConnectAsync(_emailConfiguration.SmtpHost, _emailConfiguration.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailConfiguration.SmtpUser, _emailConfiguration.SmtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}