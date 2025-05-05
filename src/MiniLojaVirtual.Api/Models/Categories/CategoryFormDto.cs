using System.ComponentModel.DataAnnotations;

namespace MiniLojaVirtual.Api.Models.Categories;

public class CategoryFormDto
{
	[Required(ErrorMessage = "O nome da categoria é obrigatório")]
	[StringLength(50, ErrorMessage = "O nome não pode exceder 50 caracteres")]
	public string Name { get; set; }

	[StringLength(100, ErrorMessage = "A descrição não pode exceder 100 caracteres")]
	public string Description { get; set; }
}