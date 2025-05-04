using MiniLojaVirtual.Domain.Models.Products;

namespace MiniLojaVirtual.Infrastructure.Entities.Products;

public class ProductEntity : Product
{
	public long CategoryId { get; set; }
	public long UserId { get; set; }

	public CategoryEntity? Category { get; set; }
	public UserEntity? User { get; set; }
}