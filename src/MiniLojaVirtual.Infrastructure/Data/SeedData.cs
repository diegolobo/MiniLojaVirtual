using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MiniLojaVirtual.Infrastructure.Contexts;
using MiniLojaVirtual.Infrastructure.Entities;
using MiniLojaVirtual.Infrastructure.Entities.Products;

namespace MiniLojaVirtual.Infrastructure.Data;

public static class SeedData
{
	public static async Task SeedAsync(IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

		// Seed categorias
		if (!context.Categories.Any())
		{
			var categories = new[]
			{
				new CategoryEntity { Name = "Eletrônicos", Description = "Produtos eletrônicos em geral" },
				new CategoryEntity { Name = "Roupas", Description = "Vestuário e acessórios" },
				new CategoryEntity { Name = "Casa e Jardim", Description = "Produtos para casa e jardim" },
				new CategoryEntity { Name = "Livros", Description = "Livros e materiais educativos" }
			};

			await context.Categories.AddRangeAsync(categories);
			await context.SaveChangesAsync();
		}

		// Seed usuário administrador
		if (!context.Users.Any())
		{
			var adminUser = new UserEntity
			{
				UserName = "admin@miniloja.com",
				Email = "admin@miniloja.com",
				EmailConfirmed = true,
				Name = "Administrador",
				PhoneNumber = "(11) 99999-9999"
			};

			var result = await userManager.CreateAsync(adminUser, "Admin123!");
			if (result.Succeeded)
			{
				await context.SaveChangesAsync();
			}
		}

		// Seed produtos
		if (!context.Products.Any())
		{
			var categories = context.Categories.ToList();
			if (categories.Count > 0)
			{
				var products = new[]
				{
					new ProductEntity
					{
						Name = "Smartphone Samsung",
						Description = "Smartphone com 128GB de armazenamento",
						Price = 899.99m,
						ImageUrl = "https://via.placeholder.com/300x200",
						CategoryId = categories.First(c => c.Name == "Eletrônicos").Id
					},
					new ProductEntity
					{
						Name = "Camiseta Polo",
						Description = "Camiseta polo masculina",
						Price = 79.90m,
						ImageUrl = "https://via.placeholder.com/300x200",
						CategoryId = categories.First(c => c.Name == "Roupas").Id
					},
					new ProductEntity
					{
						Name = "Vaso Decorativo",
						Description = "Vaso decorativo para plantas",
						Price = 45.50m,
						ImageUrl = "https://via.placeholder.com/300x200",
						CategoryId = categories.First(c => c.Name == "Casa e Jardim").Id
					}
				};

				await context.Products.AddRangeAsync(products);
				await context.SaveChangesAsync();
			}
		}
	}
}