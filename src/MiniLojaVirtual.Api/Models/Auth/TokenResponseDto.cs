namespace MiniLojaVirtual.Api.Models.Auth;

public class TokenResponseDto
{
	public string? Token { get; set; }
	public DateTime Expiration { get; set; }
}