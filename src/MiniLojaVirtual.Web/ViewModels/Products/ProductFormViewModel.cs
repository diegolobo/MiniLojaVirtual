using MiniLojaVirtual.Infrastructure.Entities.Products;
using MiniLojaVirtual.Web.Mvc.Validations;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MiniLojaVirtual.Web.ViewModels.Products;

public class ProductFormViewModel
{
	public Guid Code { get; set; }

	[DisplayName("Categoria")]
	[Required(ErrorMessage = "O campo {0} é obrigatório.")]
	public long CategoryId { get; set; }

	[DisplayName("Nome do Produto")]
	[Required(ErrorMessage = "O campo {0} é obrigatório.")]
	public string Name { get; set; } = string.Empty;

	[DisplayName("Descrição")]
	[Required(ErrorMessage = "O campo {0} é obrigatório.")]
	public string Description { get; set; } = string.Empty;

	[DisplayName("Valor")]
	[Required(ErrorMessage = "O campo {0} é obrigatório.")]
	[Range(0.01, 999999.99, ErrorMessage = "O campo {0} deve ser maior que {1} e menor que {2}.")]
	public decimal Price { get; set; }

	[DisplayName("Estoque")]
	[Required(ErrorMessage = "O campo {0} é obrigatório.")]
	[Range(1, 9999, ErrorMessage = "O campo {0} deve ser maior que {1} e menor que {2}.")]
	public int Stock { get; set; }

	[DisplayName("Foto")]
	[DataType(DataType.Upload)]
	[ValidImageUrl(ErrorMessage = "Forneça uma URL de imagem válida ou uma imagem em formato base64.")]
	public string ImageUrl { get; set; } = string.Empty;

	public static implicit operator ProductEntity(ProductFormViewModel? viewModel)
	{
		if (viewModel is null)
			return new ProductEntity
			{
				Name = string.Empty
			};

		return new ProductEntity
		{
			Code = viewModel.Code,
			CategoryId = viewModel.CategoryId,
			Name = viewModel.Name,
			Description = viewModel.Description,
			Price = viewModel.Price,
			Stock = viewModel.Stock,
			ImageUrl = viewModel.ImageUrl
		};
	}
}