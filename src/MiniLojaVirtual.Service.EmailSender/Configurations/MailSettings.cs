namespace MiniLojaVirtual.Service.EmailSender.Configurations;

public class MailSettings
{
	public string SenderName { get; set; } = string.Empty;
	public string SenderEmailAddress { get; set; } = string.Empty;
	public string RecipientName { get; set; } = string.Empty;
	public string RecipientEmailAddress { get; set; } = string.Empty;
	public string Host { get; set; } = string.Empty;
	public int Port { get; set; }
	public bool UseSsl { get; set; }
	public string User { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string LogoUrl { get; set; } = string.Empty;
}