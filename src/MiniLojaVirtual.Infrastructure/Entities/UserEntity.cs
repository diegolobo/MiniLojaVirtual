using Microsoft.AspNetCore.Identity;

using MiniLojaVirtual.Domain.Models.Abstracts.Base;

namespace MiniLojaVirtual.Infrastructure.Entities;

public class UserEntity : IdentityUser<long>, IEntity
{
	public Guid Code { get; set; }
	public required string Name { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsDeleted { get; set; }
}