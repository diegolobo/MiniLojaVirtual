using MiniLojaVirtual.Domain.Models.Products;

namespace MiniLojaVirtual.Infrastructure.Entities.Products;

public class CategoryEntity : Category
{
	public long UserId { get; set; }
	public UserEntity? User { get; set; }
	public List<ProductEntity>? Products { get; set; }
}