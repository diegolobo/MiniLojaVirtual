using MiniLojaVirtual.Domain.Models.Abstracts;

namespace MiniLojaVirtual.Domain.Models.User;

public class ApplicationUser : Entity
{
	public required string Name { get; set; }
}