using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MiniLojaVirtual.Domain.Constants;
using MiniLojaVirtual.Infrastructure.Contexts;
using MiniLojaVirtual.Infrastructure.Entities;

namespace MiniLojaVirtual.Infrastructure;

public static class DependencyInjection
{
	public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(configuration.GetConnectionString(DomainConstants.DefaultConnectionStringName)));

		services.AddDefaultIdentity<UserEntity>(options =>
			{
				options.SignIn.RequireConfirmedAccount = true;
				options.Password.RequiredLength = DomainConstants.PasswordRequiredLength;
				options.Lockout.MaxFailedAccessAttempts = DomainConstants.LockoutMaxFailedAccessAttempts;
			})
			.AddApiEndpoints()
			.AddEntityFrameworkStores<ApplicationDbContext>();
	}
}