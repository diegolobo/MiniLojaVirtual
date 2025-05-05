namespace MiniLojaVirtual.Api.Models.Categories;

public class CategoryProductDto
{
	public Guid Code { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public int Stock { get; set; }
	public string ImageUrl { get; set; }
}