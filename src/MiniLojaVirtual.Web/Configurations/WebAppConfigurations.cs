using Microsoft.AspNetCore.Localization;

using MiniLojaVirtual.Domain.Constants;
using MiniLojaVirtual.Web.Mvc;

using System.Globalization;

namespace MiniLojaVirtual.Web.Configurations;

public static class WebAppConfigurations
{
	public static void AddWebAppConfigurations(
		this IServiceCollection services,
		ConfigurationManager configuration,
		bool isDevelopment)
	{
		services.AddControllersWithViews(options =>
			{
				options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
				options.ModelBindingMessageProvider.ConfigurePortugueseErrorMessages();
			})
			.AddViewLocalization()
			.AddDataAnnotationsLocalization();

		services.AddAuthentication();

		if (!isDevelopment) return;

		services.AddDatabaseDeveloperPageExceptionFilter();
		configuration.AddUserSecrets<Program>();
	}

	public static void UseWebApp(this WebApplication app)
	{
		var supportedCultures = new[] { new CultureInfo(DomainConstants.DefaultCultureName) };
		app.UseRequestLocalization(new RequestLocalizationOptions
		{
			DefaultRequestCulture = new RequestCulture(DomainConstants.DefaultCultureName),
			SupportedCultures = supportedCultures,
			SupportedUICultures = supportedCultures
		});

		if (app.Environment.IsDevelopment())
		{
			app.UseMigrationsEndPoint();
		}
		else
		{
			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthorization();

		app.MapControllerRoute(
			"default",
			"{controller=Home}/{action=Index}/{id?}");

		app.MapRazorPages();
	}
}