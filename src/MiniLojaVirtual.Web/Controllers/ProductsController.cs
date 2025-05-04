using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using MiniLojaVirtual.Infrastructure.Contexts;
using MiniLojaVirtual.Infrastructure.Entities.Products;
using MiniLojaVirtual.Web.Controllers.Abstracts;

namespace MiniLojaVirtual.Web.Controllers;

[Route("produtos")]
[Authorize]
public class ProductsController : MainController
{
	private readonly ApplicationDbContext _context;

	public ProductsController(ApplicationDbContext context)
	{
		_context = context;
	}

	[AllowAnonymous]
	public async Task<IActionResult> Index()
	{
		var applicationDbContext = _context.Products.Include(p => p.Category).Include(p => p.User);
		return View(await applicationDbContext.ToListAsync());
	}

	[Route("{id:long}/detalhe")]
	[AllowAnonymous]
	public async Task<IActionResult> Details(long id)
	{
		var productEntity = await _context.Products
			.Include(p => p.Category)
			.Include(p => p.User)
			.FirstOrDefaultAsync(m => m.Id == id);

		if (productEntity == null) return NotFound();

		return View(productEntity);
	}

	[Route("novo")]
	public IActionResult Create()
	{
		ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
		ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(
		[Bind("CategoryId,UserId,Name,Description,Price,Stock,ImageUrl,Id,Code,CreatedAt,UpdatedAt,IsDeleted")]
		ProductEntity productEntity)
	{
		if (ModelState.IsValid)
		{
			if (CheckValidUser(productEntity, out var userId, out var view)
				|| (userId == 0
					&& view != null)) return view!;

			productEntity.UserId = userId;
			productEntity.CreatedAt = DateTime.UtcNow;

			_context.Add(productEntity);
			await _context.SaveChangesAsync();

			TempData["Success"] = $"Produto '{productEntity.Name}' cadastrado com sucesso!";
			return RedirectToAction(nameof(Index));
		}

		ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", productEntity.CategoryId);

		return View(productEntity);
	}

	[Route("{id:long}/editar")]
	public async Task<IActionResult> Edit(long id)
	{
		var productEntity = await _context.Products.FindAsync(id);
		if (productEntity == null) return NotFound();

		ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", productEntity.CategoryId);
		ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", productEntity.UserId);

		return View(productEntity);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(
		long id,
		[Bind("CategoryId,UserId,Name,Description,Price,Stock,ImageUrl,Id,Code,CreatedAt,UpdatedAt,IsDeleted")]
		ProductEntity productEntity)
	{
		if (id != productEntity.Id) return NotFound();

		if (ModelState.IsValid)
		{
			try
			{
				if (CheckValidUser(productEntity, out var userId, out var view)
					|| (userId == 0
						&& view != null)) return view!;

				if (!await UserOwnsProductAsync(productEntity.Id, userId))
				{
					ModelState.AddModelError(string.Empty, "Você não tem permissão para editar este produto.");
					return View(productEntity);
				}

				productEntity.UserId = userId;
				productEntity.UpdatedAt = DateTime.UtcNow;

				_context.Update(productEntity);

				TempData["Success"] = $"Produto '{productEntity.Name}' editado com sucesso!";
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductEntityExists(productEntity.Id))
					return NotFound();

				throw;
			}

			return RedirectToAction(nameof(Index));
		}

		ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", productEntity.CategoryId);
		ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", productEntity.UserId);
		return View(productEntity);
	}

	private bool CheckValidUser(ProductEntity productEntity, out long userId, out IActionResult? view)
	{
		userId = UserId();

		if (userId == 0)
		{
			ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
			view = View(productEntity);
			return true;
		}

		view = null;
		return false;
	}

	[Route("{id:long}/excluir")]
	public async Task<IActionResult> Delete(long id)
	{
		var productEntity = await _context.Products
			.Include(p => p.Category)
			.Include(p => p.User)
			.FirstOrDefaultAsync(m => m.Id == id);

		if (productEntity == null) return NotFound();

		return View(productEntity);
	}

	[HttpPost]
	[ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(long id)
	{
		var productEntity = await _context.Products.FindAsync(id);

		if (productEntity is null)
		{
			ModelState.AddModelError(string.Empty, "Produto não encontrado.");
			return View();
		}

		if (CheckValidUser(productEntity, out var userId, out var view)
			|| userId == 0
			|| view != null) return view!;

		if (!await UserOwnsProductAsync(productEntity.Id, userId))
		{
			ModelState.AddModelError(string.Empty, "Você não tem permissão para excluir este produto.");
			return View(productEntity);
		}

		productEntity.IsDeleted = true;
		productEntity.UpdatedAt = DateTime.UtcNow;

		_context.Products.Update(productEntity);

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private long UserId()
	{
		var userId = GetCurrentUserId();

		if (string.IsNullOrWhiteSpace(userId) || !long.TryParse(userId, out var longUserId))
			return 0;

		return longUserId;
	}

	private async Task<bool> UserOwnsProductAsync(long productId, long userId)
	{
		return await _context.Products.AsNoTracking().AnyAsync(p => p.Id == productId && p.UserId == userId);
	}

	private bool ProductEntityExists(long id)
	{
		return _context.Products.AsNoTracking().Any(e => e.Id == id);
	}
}