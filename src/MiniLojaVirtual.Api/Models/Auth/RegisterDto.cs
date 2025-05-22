using System.ComponentModel.DataAnnotations;

namespace MiniLojaVirtual.Api.Models.Auth;

public class RegisterDto
{
	[Required(ErrorMessage = "O nome é obrigatório")]
	[StringLength(100, ErrorMessage = "O nome deve ter entre {2} e {1} caracteres", MinimumLength = 3)]
	public string? Name { get; set; }

	[Required(ErrorMessage = "O email é obrigatório")]
	[EmailAddress(ErrorMessage = "Email inválido")]
	public string? Email { get; set; }

	[Required(ErrorMessage = "A senha é obrigatória")]
	[StringLength(100, ErrorMessage = "A senha deve ter entre {2} e {1} caracteres", MinimumLength = 6)]
	public string? Password { get; set; }

	[Compare("Password", ErrorMessage = "As senhas não conferem")]
	public string? ConfirmPassword { get; set; }
}