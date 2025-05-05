using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MiniLojaVirtual.Api.Controllers.Base;
using MiniLojaVirtual.Api.Models.Categories;
using MiniLojaVirtual.Domain.Extensions;
using MiniLojaVirtual.Infrastructure.Contexts;
using MiniLojaVirtual.Infrastructure.Entities.Products;

using System.Net;

namespace MiniLojaVirtual.Api.Controllers;

[Authorize]
public class CategoriesController : BaseApiController
{
	private readonly ApplicationDbContext _context;

	public CategoriesController(
		ILogger<CategoriesController> logger,
		ApplicationDbContext context)
		: base(logger)
	{
		_context = context;
	}

	[HttpGet]
	[AllowAnonymous]
	[ProducesResponseType(typeof(IEnumerable<CategoryItemDto>), (int)HttpStatusCode.OK)]
	public async Task<IActionResult> GetAll()
	{
		var categories = await _context.Categories
			.AsNoTracking()
			.Where(c => !c.IsDeleted)
			.Select(c => new CategoryItemDto
			{
				Code = c.Code,
				Name = c.Name,
				Description = c.Description
			})
			.ToListAsync();

		return Ok(categories);
	}

	[HttpGet("{code:guid}")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(CategoryDetailsDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.NotFound)]
	public async Task<IActionResult> GetById(Guid code)
	{
		var category = await _context.Categories
			.AsNoTracking()
			.Include(c => c.Products.Where(p => !p.IsDeleted))
			.Where(c => c.Code == code && !c.IsDeleted)
			.Select(c => new CategoryDetailsDto
			{
				Code = c.Code,
				Name = c.Name,
				Description = c.Description,
				Products = c.Products.Select(p => new CategoryProductDto
				{
					Code = p.Code,
					Name = p.Name,
					Price = p.Price,
					Stock = p.Stock,
					ImageUrl = p.ImageUrl
				}).ToList()
			})
			.FirstOrDefaultAsync();

		if (category == null)
			return NotFound(new { message = "Categoria não encontrada" });

		return Ok(category);
	}

	[HttpPost]
	[ProducesResponseType(typeof(CategoryItemDto), (int)HttpStatusCode.Created)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	public async Task<IActionResult> Create([FromBody] CategoryFormDto categoryDto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		try
		{
			var userId = GetUserId();
			if (userId == 0)
				return Unauthorized(new { message = "Usuário não autenticado" });

			var category = new CategoryEntity
			{
				Code = string.Empty.NewUuidV7(),
				Name = categoryDto.Name,
				Description = categoryDto.Description,
				UserId = userId,
				CreatedAt = DateTime.UtcNow
			};

			_context.Add(category);
			await _context.SaveChangesAsync();

			var result = new CategoryItemDto
			{
				Code = category.Code,
				Name = category.Name,
				Description = category.Description
			};

			return CreatedAtAction(nameof(GetById), new { code = result.Code }, result);
		}
		catch (Exception ex)
		{
			Logger.LogError($"Erro ao criar categoria: {ex.Message}");
			return BadRequest(new { message = "Erro ao criar categoria" });
		}
	}

	[HttpPut("{code:guid}")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.NotFound)]
	public async Task<IActionResult> Update(Guid code, [FromBody] CategoryFormDto categoryDto)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var category = await _context.Categories.FirstOrDefaultAsync(c => c.Code == code && !c.IsDeleted);

		if (category == null)
			return NotFound(new { message = "Categoria não encontrada" });

		var userId = GetUserId();
		if (userId == 0)
			return Unauthorized(new { message = "Usuário não autenticado" });

		if (category.UserId != userId)
			return Forbid();

		try
		{
			category.Name = categoryDto.Name;
			category.Description = categoryDto.Description;
			category.UpdatedAt = DateTime.UtcNow;

			_context.Update(category);
			await _context.SaveChangesAsync();

			return NoContent();
		}
		catch (Exception ex)
		{
			Logger.LogError($"Erro ao atualizar categoria: {ex.Message}");
			return BadRequest(new { message = "Erro ao atualizar categoria" });
		}
	}

	[HttpDelete("{code:guid}")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.NotFound)]
	[ProducesResponseType((int)HttpStatusCode.BadRequest)]
	public async Task<IActionResult> Delete(Guid code)
	{
		var category = await _context.Categories
			.Include(c => c.Products.Where(p => !p.IsDeleted))
			.FirstOrDefaultAsync(c => c.Code == code && !c.IsDeleted);

		if (category == null)
			return NotFound(new { message = "Categoria não encontrada" });

		var userId = GetUserId();
		if (userId == 0)
			return Unauthorized(new { message = "Usuário não autenticado" });

		if (category.UserId != userId)
			return Forbid();

		if (category.Products.Any())
			return BadRequest(new
			{ message = "Não é possível excluir esta categoria pois existem produtos associados a ela" });

		try
		{
			category.IsDeleted = true;
			category.UpdatedAt = DateTime.UtcNow;

			_context.Update(category);
			await _context.SaveChangesAsync();

			return NoContent();
		}
		catch (Exception ex)
		{
			Logger.LogError($"Erro ao excluir categoria: {ex.Message}");
			return BadRequest(new { message = "Erro ao excluir categoria" });
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