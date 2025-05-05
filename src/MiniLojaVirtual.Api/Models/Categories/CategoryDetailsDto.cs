namespace MiniLojaVirtual.Api.Models.Categories;

public class CategoryDetailsDto : CategoryItemDto
{
	public IEnumerable<CategoryProductDto> Products { get; set; } = [];
}