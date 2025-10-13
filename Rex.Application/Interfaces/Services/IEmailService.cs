using Rex.Application.DTOs.Configs;

namespace Rex.Application.Interfaces;

/// <summary>
/// Service for sending emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email using the provided email data.
    /// </summary>
    Task SendEmailAsync(EmailDto emailAnswer);
}