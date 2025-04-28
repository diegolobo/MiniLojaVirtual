using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using MimeKit;

using MiniLojaVirtual.Infrastructure.Entities;
using MiniLojaVirtual.Service.EmailSender.Configurations;
using MiniLojaVirtual.Service.EmailSender.Constants;
using MiniLojaVirtual.Service.EmailSender.Enums;

using static System.String;

namespace MiniLojaVirtual.Service.EmailSender;

internal class EmailSender : IEmailSender<UserEntity>
{
	private readonly MailSettings _mailSettings;

	public EmailSender(IOptions<MailSettings> mailSettings)
	{
		_mailSettings = mailSettings.Value;
	}

	public async Task SendConfirmationLinkAsync(UserEntity user, string email, string confirmationLink)
	{
		CheckSendEmail(user, confirmationLink, EmailSendType.ConfirmationLink);

		await SendEmailAsync(
			user.Email!,
			EmailTemplates.EmailConfirmationLinkSubject,
			EmailTemplates.EmailConfirmationLinkHtmlMessage(_mailSettings.LogoUrl, confirmationLink),
			user.Name,
			email);
	}

	public async Task SendPasswordResetLinkAsync(UserEntity user, string email, string resetLink)
	{
		CheckSendEmail(user, resetLink, EmailSendType.PasswordResetLink);

		await SendEmailAsync(
			user.Email!,
			EmailTemplates.EmailPasswordResetLinkSubject,
			EmailTemplates.EmailPasswordResetLinkHtmlMessage(_mailSettings.LogoUrl, resetLink),
			user.Name,
			email);
	}

	public async Task SendPasswordResetCodeAsync(UserEntity user, string email, string resetCode)
	{
		CheckSendEmail(user, resetCode, EmailSendType.PasswordResetCode);

		await SendEmailAsync(
			user.Email!,
			EmailTemplates.EmailPasswordResetCodeSubject,
			EmailTemplates.EmailPasswordResetCodeHtmlMessage(_mailSettings.LogoUrl, resetCode),
			user.Name,
			email);
	}

	private async Task SendEmailAsync(string emailAddress, string subject, string htmlMessage, string name,
		string? additionalEmailAddress = null)
	{
		var emailMessage = new MimeMessage();
		emailMessage.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmailAddress));
		emailMessage.Bcc.Add(new MailboxAddress(_mailSettings.RecipientName, _mailSettings.RecipientEmailAddress));
		emailMessage.To.Add(new MailboxAddress(name, emailAddress));

		if (!IsNullOrWhiteSpace(additionalEmailAddress) && !emailAddress.Equals(additionalEmailAddress))
			emailMessage.Cc.Add(new MailboxAddress(name, additionalEmailAddress));

		emailMessage.Subject = subject;
		emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
		{
			Text = htmlMessage
		};

		using var smtp = new SmtpClient();
		await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, _mailSettings.UseSsl);
		await smtp.AuthenticateAsync(_mailSettings.User, _mailSettings.Password);
		await smtp.SendAsync(emailMessage);
		await smtp.DisconnectAsync(true);
	}

	private static void CheckSendEmail(UserEntity user, string link, EmailSendType type)
	{
		if (IsNullOrWhiteSpace(user.Email))
			throw new InvalidOperationException(EmailTemplates.UserEmailIsRequiredMessage);

		if (IsNullOrWhiteSpace(user.Name))
			throw new InvalidOperationException(EmailTemplates.UserNameIsRequiredMessage);

		if (IsNullOrWhiteSpace(link))
			throw new InvalidOperationException(Format(EmailTemplates.LinkIsRequiredMessage, GetSendTypeName(type)));
	}

	private static string GetSendTypeName(EmailSendType type)
	{
		return type switch
		{
			EmailSendType.ConfirmationLink => EmailTemplates.ConfirmationLink,
			EmailSendType.PasswordResetLink => EmailTemplates.PasswordResetLink,
			EmailSendType.PasswordResetCode => EmailTemplates.PasswordResetCode,
			_ => EmailTemplates.Link
		};
	}
}