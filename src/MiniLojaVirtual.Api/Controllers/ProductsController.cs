using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MiniLojaVirtual.Api.Controllers.Base;
using MiniLojaVirtual.Api.Models.Products;
using MiniLojaVirtual.Domain.Extensions;
using MiniLojaVirtual.Infrastructure.Contexts;
using MiniLojaVirtual.Infrastructure.Entities.Products;

using System.Net;

namespace MiniLojaVirtual.Api.Controllers;

[Authorize]
public class ProductsController : BaseApiController
{
	private readonly ApplicationDbContext _context;

	public ProductsController(
		ILogger<ProductsController> logger,
		ApplicationDbContext context)
		: base(logger)
	{
		_context = context;
	}

	[HttpGet]
	[AllowAnonymous]
	[ProducesResponseType(typeof(IEnumerable<ProductItemDto>), (int)HttpStatusCode.OK)]
	public async Task<IActionResult> GetAll()
	{
		var products = await _context.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.Include(p => p.User)
			.Where(p => !p.IsDeleted)
			.Select(p => new ProductItemDto
			{
				Name = p.Name,
				Code = p.Code,
				CategoryId = p.CategoryId,
				CategoryName = p.Category == null ? "Padrão" : p.Category.Name,
				Description = p.Description,
				Price = p.Price,
				Stock = p.Stock,
				ImageUrl = p.ImageUrl
			})
			.ToListAsync();

		return Ok(products);
	}

	[HttpGet("{code:guid}")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(ProductItemDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.NotFound)]
	public async Task<IActionResult> GetById(Guid code)
	{
		var product = await _context.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.Include(p => p.User)
			.Where(p => p.Code == code && !p.IsDeleted)
			.Select(p => new ProductItemDto
			{
				Name = p.Name,
				Code = p.Code,
				CategoryId = p.CategoryId,
				CategoryName = p.Category == null ? "Padrão" : p.Category.Name,
				Description = p.Description,
				Price = p.Price,
				Stock = p.Stock,
				ImageUrl = p.ImageUrl
			})
			.FirstOrDefaultAsync();

		if (product == null)
			return NotFound(new { message = "Produto não encontrado" });

		return Ok(product);
	}

	[HttpPost]
	[ProducesResponseType(typeof(ProductItemDto), (int)HttpStatusCode.Created)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	public async Task<IActionResult> Create([FromBody] ProductFormDto productDto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		try
		{
			productDto.ImageUrl = await SaveImage(productDto.ImageUrl);

			var userId = GetUserId();
			if (userId == 0)
				return Unauthorized(new { message = "Usuário não autenticado" });

			var product = new ProductEntity
			{
				Code = string.Empty.NewUuidV7(),
				Name = productDto.Name,
				Description = productDto.Description,
				Price = productDto.Price,
				Stock = productDto.Stock,
				ImageUrl = productDto.ImageUrl,
				CategoryId = productDto.CategoryId,
				UserId = userId,
				CreatedAt = DateTime.UtcNow
			};

			_context.Add(product);
			await _context.SaveChangesAsync();

			var result = new ProductItemDto
			{
				Code = product.Code,
				Name = product.Name,
				Description = product.Description,
				Price = product.Price,
				Stock = product.Stock,
				ImageUrl = product.ImageUrl,
				CategoryId = product.CategoryId,
				CategoryName = product.Category?.Name ?? "Padrão"
			};

			return CreatedAtAction(nameof(GetById), new { code = result.Code }, result);
		}
		catch (Exception ex)
		{
			Logger.LogError($"Erro ao criar produto: {ex.Message}");
			return BadRequest(new { message = "Erro ao criar produto" });
		}
	}

	[HttpPut("{code:guid}")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.NotFound)]
	public async Task<IActionResult> Update(Guid code, [FromBody] ProductFormDto productDto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var product = await _context.Products.FirstOrDefaultAsync(p => p.Code == code && !p.IsDeleted);

		if (product == null)
			return NotFound(new { message = "Produto não encontrado" });

		var userId = GetUserId();
		if (userId == 0)
			return Unauthorized(new { message = "Usuário não autenticado" });

		if (product.UserId != userId)
			return Forbid();

		try
		{
			product.Name = productDto.Name;
			product.Description = productDto.Description;
			product.Price = productDto.Price;
			product.Stock = productDto.Stock;
			product.ImageUrl = await SaveImage(productDto.ImageUrl);
			product.CategoryId = productDto.CategoryId;
			product.UpdatedAt = DateTime.UtcNow;

			_context.Update(product);
			await _context.SaveChangesAsync();

			return NoContent();
		}
		catch (Exception ex)
		{
			Logger.LogError($"Erro ao atualizar produto: {ex.Message}");
			return BadRequest(new { message = "Erro ao atualizar produto" });
		}
	}

	[HttpDelete("{code:guid}")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.NotFound)]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType((int)HttpStatusCode.Forbidden)]
	public async Task<IActionResult> Delete(Guid code)
	{
		var product = await _context.Products.FirstOrDefaultAsync(p => p.Code == code && !p.IsDeleted);

		if (product == null)
			return NotFound(new { message = "Produto não encontrado" });

		var userId = GetUserId();
		if (userId == 0)
			return Unauthorized(new { message = "Usuário não autenticado" });

		if (product.UserId != userId)
			return Forbid();

		try
		{
			product.IsDeleted = true;
			product.UpdatedAt = DateTime.UtcNow;

			_context.Update(product);
			await _context.SaveChangesAsync();

			return NoContent();
		}
		catch (Exception ex)
		{
			Logger.LogError($"Erro ao excluir produto: {ex.Message}");
			return BadRequest(new { message = "Erro ao excluir produto" });
		}
	}

	private long GetUserId()
	{
		var userId = GetCurrentUserId();

		if (string.IsNullOrWhiteSpace(userId) || !long.TryParse(userId, out var longUserId))
			return 0;

		return longUserId;
	}
}