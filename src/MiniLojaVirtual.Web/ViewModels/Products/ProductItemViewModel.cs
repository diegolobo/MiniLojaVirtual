using MiniLojaVirtual.Infrastructure.Entities.Products;

namespace MiniLojaVirtual.Web.ViewModels.Products;

public class ProductItemViewModel
{
	public Guid Code { get; set; }

	public string? SellerName { get; set; }

	public string? CategoryName { get; set; }

	public long CategoryId { get; set; }

	public string? Name { get; set; }

	public string? Description { get; set; }

	public decimal Price { get; set; }

	public int Stock { get; set; }

	public string? ImageUrl { get; set; }

	public static implicit operator ProductItemViewModel(ProductEntity? entity)
	{
		if (entity is null)
			return new ProductItemViewModel();

		return new ProductItemViewModel
		{
			Code = entity.Code,
			SellerName = entity.User?.Name ?? entity.User?.Email ?? string.Empty,
			CategoryName = entity.Category?.Name ?? string.Empty,
			CategoryId = entity.CategoryId,
			Name = entity.Name,
			Description = entity.Description ?? string.Empty,
			Price = entity.Price,
			Stock = entity.Stock,
			ImageUrl = entity.ImageUrl ?? string.Empty
		};
	}
}