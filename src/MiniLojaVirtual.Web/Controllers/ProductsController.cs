using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using MiniLojaVirtual.Infrastructure.Contexts;
using MiniLojaVirtual.Infrastructure.Entities.Products;

namespace MiniLojaVirtual.Web.Controllers;

[Route("produtos")]
[Authorize]
public class ProductsController : Controller
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
			_context.Add(productEntity);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", productEntity.CategoryId);
		ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", productEntity.UserId);

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
				_context.Update(productEntity);
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
		if (productEntity != null) _context.Products.Remove(productEntity);

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool ProductEntityExists(long id)
	{
		return _context.Products.Any(e => e.Id == id);
	}
}