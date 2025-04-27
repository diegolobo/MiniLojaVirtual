using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using MiniLojaVirtual.Infrastructure.Entities;

namespace MiniLojaVirtual.Infrastructure.Contexts;

public class ApplicationDbContext : IdentityDbContext<UserEntity>
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
}