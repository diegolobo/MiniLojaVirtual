namespace MiniLojaVirtual.Api.Models.Products;

public class ProductItemDto
{
	public Guid Code { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public decimal Price { get; set; }
	public int Stock { get; set; }
	public string ImageUrl { get; set; }
	public long CategoryId { get; set; }
	public string CategoryName { get; set; }
}