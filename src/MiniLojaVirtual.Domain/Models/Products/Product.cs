using MiniLojaVirtual.Domain.Models.Abstracts;

namespace MiniLojaVirtual.Domain.Models.Products;

public class Product : Entity
{
	public required string Name { get; set; }
	public string? Description { get; set; }
	public decimal Price { get; set; }
	public int Stock { get; set; }
	public string? ImageUrl { get; set; }
}