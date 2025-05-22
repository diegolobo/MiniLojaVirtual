using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MiniLojaVirtual.Application.Mvc.Validations;

public class ValidImageUrlAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
			return ValidationResult.Success;

		var imageUrl = value.ToString();

		var urlRegex = new Regex(@"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$",
			RegexOptions.IgnoreCase);

		var base64ImageRegex = new Regex(@"^data:image\/(jpeg|jpg|png|gif|bmp|webp);base64,",
			RegexOptions.IgnoreCase);

		if (urlRegex.IsMatch(imageUrl) || base64ImageRegex.IsMatch(imageUrl)) return ValidationResult.Success;

		return new ValidationResult(
			ErrorMessage ??
			"O formato da URL da imagem é inválido. Forneça uma URL válida ou uma imagem em formato base64.");
	}
}