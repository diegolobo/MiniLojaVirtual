using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using MiniLojaVirtual.Infrastructure.Entities;
using MiniLojaVirtual.Infrastructure.Entities.Products;

namespace MiniLojaVirtual.Infrastructure.Contexts;

public class ApplicationDbContext : IdentityDbContext<UserEntity, IdentityRole<long>, long>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
	}

	#region DbSet's

	public DbSet<ProductEntity> Products { get; set; }
	public DbSet<CategoryEntity> Categories { get; set; }

	#endregion
}