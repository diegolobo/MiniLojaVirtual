using MiniLojaVirtual.Domain.Models.Abstracts;

namespace MiniLojaVirtual.Domain.Models.Users;

public class ApplicationUser : Entity
{
	public required string Name { get; set; }
}