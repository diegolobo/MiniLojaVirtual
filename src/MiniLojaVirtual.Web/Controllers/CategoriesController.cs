using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MiniLojaVirtual.Infrastructure.Contexts;
using MiniLojaVirtual.Infrastructure.Entities.Products;
using MiniLojaVirtual.Web.Controllers.Base;

namespace MiniLojaVirtual.Web.Controllers;

[Route("categorias")]
[Authorize]
public class CategoriesController : MainController
{
	private readonly ApplicationDbContext _context;

	public CategoriesController(
		ILogger<MainController> logger,
		ApplicationDbContext context)
		: base(logger)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
		var categories = _context.Categories.Include(c => c.User);
		return View(await categories.ToListAsync());
	}

	[Route("{code:Guid}/detalhes")]
	public async Task<IActionResult> Details(Guid code)
	{
		var categoryEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Code == code);
		if (categoryEntity == null) return NotFound();

		if (CheckValidUser(categoryEntity, out var userId, out var view)
			|| (userId == 0
				&& view != null)) return view!;

		return View(categoryEntity);
	}

	[Route("nova")]
	public IActionResult Create()
	{
		return View();
	}

	[HttpPost("nova")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(
		[Bind("Name,Description")] CategoryEntity categoryEntity)
	{
		if (ModelState.IsValid)
		{
			if (CheckValidUser(categoryEntity, out var userId, out var view)
				|| (userId == 0
					&& view != null)) return view!;

			categoryEntity.UserId = userId;
			categoryEntity.CreatedAt = DateTime.UtcNow;

			_context.Add(categoryEntity);
			await _context.SaveChangesAsync();

			TempData["Success"] = $"Categoria '{categoryEntity.Name}' cadastrada com sucesso!";
			return RedirectToAction(nameof(Index));
		}

		return View(categoryEntity);
	}

	[Route("{code:Guid}/editar")]
	public async Task<IActionResult> Edit(Guid code)
	{
		var categoryEntity = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Code == code);
		if (categoryEntity == null) return NotFound();

		var userId = UserId();
		if (!await UserOwnsCategoryAsync(categoryEntity.Id, userId))
		{
			TempData["Error"] = "Você não tem permissão para editar esta categoria.";
			return RedirectToAction(nameof(Index));
		}

		return View(categoryEntity);
	}

	[HttpPost("{code:Guid}/editar")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(
		Guid code,
		[Bind("Name,Description")] CategoryEntity category)
	{
		if (code != category.Code)
		{
			TempData["Error"] = "Código da categoria inválido.";
			Logger.LogError("Código da categoria inválido: {0}.", category.Code);
			return RedirectToAction(nameof(Index));
		}

		if (!ModelState.IsValid) return View(category);

		try
		{
			if (CheckValidUser(category, out var userId, out var view)
				|| (userId == 0
					&& view != null)) return view!;

			if (!await UserOwnsCategoryAsync(category.Id, userId))
			{
				ModelState.AddModelError(string.Empty, "Você não tem permissão para editar esta categoria.");
				return View(category);
			}

			var originalCategory = await _context.Categories.AsNoTracking()
				.FirstOrDefaultAsync(c => c.Code == code);

			category.UserId = userId;
			category.UpdatedAt = DateTime.UtcNow;
			category.CreatedAt = originalCategory!.CreatedAt;

			_context.Update(category);

			TempData["Success"] = $"Categoria '{category.Name}' editada com sucesso!";
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException dbEx)
		{
			Logger.LogError(dbEx, "Erro ao editar categoria: {0}", dbEx.Message);
			TempData["Error"] = "Erro ao editar categoria. Tente novamente mais tarde.";
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Erro ao editar categoria: {0}", ex.Message);
			TempData["Error"] = "Erro ao editar categoria. Tente novamente mais tarde.";
		}

		return RedirectToAction(nameof(Index));
	}

	[Route("{code:Guid}/excluir")]
	public async Task<IActionResult> Delete(Guid code)
	{
		var categoryEntity = await _context.Categories
			.Include(c => c.User)
			.FirstOrDefaultAsync(m => m.Code == code);

		if (categoryEntity == null) return NotFound();

		var userId = UserId();
		if (!await UserOwnsCategoryAsync(categoryEntity.Id, userId))
		{
			TempData["Error"] = "Você não tem permissão para excluir esta categoria.";
			return RedirectToAction(nameof(Index));
		}

		var hasProducts = await _context.Products.AnyAsync(p => p.Category != null && p.Category.Code == code);
		if (hasProducts)
		{
			TempData["Error"] = "Não é possível excluir esta categoria pois existem produtos associados a ela.";
			return RedirectToAction(nameof(Index));
		}

		return View(categoryEntity);
	}

	[HttpPost("{code:Guid}/excluir")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(Guid code)
	{
		var categoryEntity = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Code == code);

		if (categoryEntity is null)
		{
			ModelState.AddModelError(string.Empty, "Categoria não encontrada.");
			return RedirectToAction("Delete");
		}

		if (CheckValidUser(categoryEntity, out var userId, out var view)
			|| userId == 0
			|| view != null) return view!;

		if (!await UserOwnsCategoryAsync(categoryEntity.Id, userId))
		{
			ModelState.AddModelError(string.Empty, "Você não tem permissão para excluir esta categoria.");
			return RedirectToAction(
				"Delete",
				"Categories",
				categoryEntity);
		}

		var hasProducts = await _context.Products.AsNoTracking()
			.AnyAsync(p => p.Category != null && p.Category.Code == code);
		if (hasProducts)
		{
			ModelState.AddModelError(string.Empty,
				"Não é possível excluir esta categoria pois existem produtos associados a ela.");

			return RedirectToAction(
				"Delete",
				"Categories",
				categoryEntity);
		}

		categoryEntity.IsDeleted = true;
		categoryEntity.UpdatedAt = DateTime.UtcNow;

		_context.Categories.Update(categoryEntity);
		await _context.SaveChangesAsync();

		TempData["Success"] = $"Categoria '{categoryEntity.Name}' excluída com sucesso!";
		return RedirectToAction(nameof(Index));
	}

	private long UserId()
	{
		var userId = GetCurrentUserId();

		if (string.IsNullOrWhiteSpace(userId) || !long.TryParse(userId, out var longUserId))
			return 0;

		return longUserId;
	}

	private async Task<bool> UserOwnsCategoryAsync(long categoryId, long userId)
	{
		return await _context.Categories.AsNoTracking().AnyAsync(c => c.Id == categoryId && c.UserId == userId);
	}

	private bool CheckValidUser(CategoryEntity categoryEntity, out long userId, out IActionResult? view)
	{
		userId = UserId();

		if (userId == 0)
		{
			ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
			view = View(categoryEntity);
			return true;
		}

		view = null;
		return false;
	}
}