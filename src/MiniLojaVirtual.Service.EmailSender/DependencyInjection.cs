using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MiniLojaVirtual.Infrastructure.Entities;
using MiniLojaVirtual.Service.EmailSender.Configurations;

namespace MiniLojaVirtual.Service.EmailSender;

public static class DependencyInjection
{
	public static void AddEmailService(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
		services.AddTransient<IEmailSender<UserEntity>, EmailSender>();
	}
}