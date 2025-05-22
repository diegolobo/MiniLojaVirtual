using Microsoft.AspNetCore.Identity;

using MiniLojaVirtual.Domain.Models.Abstracts.Base;
using MiniLojaVirtual.Infrastructure.Entities.Products;

namespace MiniLojaVirtual.Infrastructure.Entities;

public class UserEntity : IdentityUser<long>, IEntity
{
	public Guid Code { get; set; }
	public string? Name { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsDeleted { get; set; }

	public virtual List<ProductEntity> Products { get; set; } = [];
	public virtual List<CategoryEntity> Categories { get; set; } = [];
}