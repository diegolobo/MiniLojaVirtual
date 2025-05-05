using System.ComponentModel.DataAnnotations;

namespace MiniLojaVirtual.Api.Models.Products;

public class ProductFormDto
{
	[Required(ErrorMessage = "O nome do produto é obrigatório")]
	[StringLength(75, ErrorMessage = "O nome não pode exceder 75 caracteres")]
	public string Name { get; set; }

	[StringLength(150, ErrorMessage = "A descrição não pode exceder 150 caracteres")]
	public string Description { get; set; }

	[Required(ErrorMessage = "O preço é obrigatório")]
	[Range(0.01, 99999.99, ErrorMessage = "O preço deve estar entre 0,01 e 99.999,99")]
	public decimal Price { get; set; }

	[Required(ErrorMessage = "O estoque é obrigatório")]
	[Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo")]
	public int Stock { get; set; }

	public string ImageUrl { get; set; }

	[Required(ErrorMessage = "A categoria é obrigatória")]
	public long CategoryId { get; set; }
}