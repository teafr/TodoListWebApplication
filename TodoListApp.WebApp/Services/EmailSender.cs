using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace TodoListApp.WebApp.Services;

public class EmailSender : IEmailSender
{
    private readonly SmtpClient smtpClient;
    private readonly MailAddress sender;

    public EmailSender(IOptions<MessageSenderOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.sender = new MailAddress(options.Value.SenderEmail!, options.Value.Sender);
        this.smtpClient = new SmtpClient(options.Value.Host, options.Value.Port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(options.Value.SenderEmail, options.Value.SenderPassword),
        };
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        MailAddress reciver = new MailAddress(email);
        using MailMessage mailMessage = new MailMessage(this.sender, reciver)
        {
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
        };

        await this.smtpClient.SendMailAsync(mailMessage);
    }
}
