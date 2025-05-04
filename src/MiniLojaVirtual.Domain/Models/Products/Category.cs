using MiniLojaVirtual.Domain.Models.Abstracts;

namespace MiniLojaVirtual.Domain.Models.Products;

public class Category : Entity
{
	public required string Name { get; set; }
	public string? Description { get; set; }
}