using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using MiniLojaVirtual.Domain.Extensions;
using MiniLojaVirtual.Infrastructure.Contexts;
using MiniLojaVirtual.Infrastructure.Entities.Products;
using MiniLojaVirtual.Web.Controllers.Abstracts;
using MiniLojaVirtual.Web.ViewModels.Products;

namespace MiniLojaVirtual.Web.Controllers;

[Route("produtos")]
[Authorize]
public class ProductsController : MainController
{
	private readonly ApplicationDbContext _context;

	public ProductsController(
		ILogger<ProductsController> logger,
		ApplicationDbContext context)
		: base(logger)
	{
		_context = context;
	}

	[AllowAnonymous]
	public async Task<IActionResult> Index()
	{
		var products = await _context.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.Include(p => p.User)
			.Select(p => new ProductItemViewModel
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

		return View(products);
	}

	[Route("{code:Guid}/detalhes")]
	[AllowAnonymous]
	public async Task<IActionResult> Details(Guid code)
	{
		var products = await _context.Products
			.Include(p => p.Category)
			.Include(p => p.User)
			.Select(p => new ProductItemViewModel
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
			.FirstOrDefaultAsync(m => m.Code == code);

		if (products == null) return ProductNotFound();

		return View(products);
	}

	[Route("novo")]
	public IActionResult Create()
	{
		ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
		return View();
	}

	[HttpPost("novo")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(
		[Bind("CategoryId,Name,Description,Price,Stock,ImageUrl")]
		ProductFormViewModel viewmodel)
	{
		if (ModelState.IsValid)
			try
			{
				viewmodel.ImageUrl = await SaveImage(viewmodel.ImageUrl);

				if (CheckValidUser(viewmodel, out var userId, out var view)
					|| (userId == 0
						&& view != null)) return view!;

				viewmodel.Code = string.Empty.NewUuidV7();
				ProductEntity product = viewmodel;
				product.UserId = userId;
				product.CreatedAt = DateTime.UtcNow;

				_context.Add(product);
				await _context.SaveChangesAsync();

				TempData["Success"] = $"Produto '{viewmodel.Name}' cadastrado com sucesso!";
				return RedirectToAction(nameof(Index), viewmodel);
			}
			catch (Exception e)
			{
				Logger.LogError("Erro ao criar o produto: {0}. {1}", viewmodel.Name, e.Message);
				TempData["Error"] = "Erro ao criar o produto, tente novamente.";
				return RedirectToAction(nameof(Index), viewmodel);
			}

		ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", viewmodel.CategoryId);

		return View(viewmodel);
	}

	[Route("{code:Guid}/editar")]
	public async Task<IActionResult> Edit(Guid code)
	{
		var product = await _context.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.Include(p => p.User)
			.Where(p => p.Code == code)
			.Select(p => new ProductFormViewModel
			{
				Name = p.Name,
				Code = p.Code,
				CategoryId = p.CategoryId,
				Description = p.Description ?? string.Empty,
				Price = p.Price,
				Stock = p.Stock,
				ImageUrl = p.ImageUrl ?? string.Empty
			})
			.FirstOrDefaultAsync();

		if (product == null) return ProductNotFound();

		ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

		return View(product);
	}

	[HttpPost("{code:Guid}/editar")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(
		Guid code,
		[Bind("CategoryId,Name,Description,Price,Stock,ImageUrl")]
		ProductFormViewModel viewmodel)
	{
		if (code != viewmodel.Code)
		{
			TempData["Error"] = "Código do produto inválido.";
			Logger.LogError("Código do produto inválido: {0}.", viewmodel.Code);
			return RedirectToAction(nameof(Index));
		}

		if (!ModelState.IsValid)
		{
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", viewmodel.CategoryId);
			return View(viewmodel);
		}

		try
		{
			if (CheckValidUser(viewmodel, out var userId, out var view)
				|| (userId == 0
					&& view != null)) return view!;

			var product = await _context.Products.FindAsync(code);

			if (product == null)
				return ProductNotFound();

			if (!await UserOwnsProductAsync(code, userId))
			{
				ModelState.AddModelError(string.Empty, "Você não tem permissão para editar este produto.");
				return View(viewmodel);
			}

			product.ImageUrl = await SaveImage(viewmodel.ImageUrl);
			product.UserId = userId;
			product.UpdatedAt = DateTime.UtcNow;

			_context.Update(product);

			TempData["Success"] = $"Produto '{product.Name}' editado com sucesso!";
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException dbEx)
		{
			Logger.LogError("Erro ao editar o produto: {0}. {1}", viewmodel.Name, dbEx.Message);
			TempData["Error"] = "Erro ao conectar com servidor, tente mais tarde.";
		}
		catch (Exception ex)
		{
			Logger.LogError("Erro ao editar o produto: {0}. {1}", viewmodel.Name, ex.Message);
			TempData["Error"] = "Erro ao editar o produto, tente novamente.";
		}

		return RedirectToAction(nameof(Index));
	}

	[Route("{code:Guid}/excluir")]
	public async Task<IActionResult> Delete(Guid code)
	{
		var product = await _context.Products
			.AsNoTracking()
			.Include(p => p.Category)
			.Include(p => p.User)
			.Where(p => p.Code == code)
			.Select(p => new ProductItemViewModel
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
			return ProductNotFound();

		return View(product);
	}

	[HttpPost]
	[ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(long id)
	{
		var product = await _context.Products.FindAsync(id);

		if (product is null)
			return ProductNotFound();

		if (CheckValidUser(product, out var userId, out var view)
			|| userId == 0
			|| view != null) return view!;

		if (!await UserOwnsProductAsync(product.Code, userId))
		{
			TempData["Error"] = "Você não tem permissão para excluir este produto.";
			return RedirectToAction(nameof(Index));
		}

		product.IsDeleted = true;
		product.UpdatedAt = DateTime.UtcNow;

		_context.Products.Update(product);

		await _context.SaveChangesAsync();
		TempData["Success"] = $"Produto '{product.Name}' excluído com sucesso!";
		return RedirectToAction(nameof(Index));
	}

	private IActionResult ProductNotFound()
	{
		TempData["Error"] = "Produto não encontrado.";
		return RedirectToAction(nameof(Index));
	}

	private long UserId()
	{
		var userId = GetCurrentUserId();

		if (string.IsNullOrWhiteSpace(userId) || !long.TryParse(userId, out var longUserId))
			return 0;

		return longUserId;
	}

	private async Task<bool> UserOwnsProductAsync(Guid code, long userId)
	{
		return await _context.Products.AsNoTracking().AnyAsync(p => p.Code == code && p.UserId == userId);
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
}