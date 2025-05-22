using System.ComponentModel.DataAnnotations;

namespace MiniLojaVirtual.Api.Models.Auth;

public class LoginDto
{
	[Required(ErrorMessage = "O email é obrigatório")]
	[EmailAddress(ErrorMessage = "Email inválido")]
	public string Email { get; set; }

	[Required(ErrorMessage = "A senha é obrigatória")]
	public string Password { get; set; }
}