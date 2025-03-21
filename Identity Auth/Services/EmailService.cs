using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
	private readonly IConfiguration _configuration;

	public EmailService(IConfiguration config)
	{
		_configuration = config;
	}

	public async Task SendEmailAsync(string toEmail, string subject, string body)
	{
		if (string.IsNullOrWhiteSpace(toEmail))
		{
			throw new ArgumentException("Recipient email cannot be empty", nameof(toEmail));
		}

		var email = new MimeMessage();
		email.From.Add(new MailboxAddress("Entity Auth", _configuration["SmtpSettings:SenderEmail"]));
		email.To.Add(new MailboxAddress(toEmail, toEmail));
		email.Subject = subject;

		var bodyBuilder = new BodyBuilder { HtmlBody = body };
		email.Body = bodyBuilder.ToMessageBody();

		using var smtp = new SmtpClient();
		await smtp.ConnectAsync(_configuration["SmtpSettings:Server"], int.Parse(_configuration["SmtpSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);

		await smtp.AuthenticateAsync(_configuration["SmtpSettings:SenderEmail"], _configuration["SmtpSettings:SenderPassword"]);
		await smtp.SendAsync(email);
		await smtp.DisconnectAsync(true);
	}

}
