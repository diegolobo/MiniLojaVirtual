using Microsoft.EntityFrameworkCore;

using MiniLojaVirtual.Infrastructure;
using MiniLojaVirtual.Service.EmailSender;
using MiniLojaVirtual.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebAppConfigurations(
	builder.Configuration,
	builder.Environment.IsDevelopment());
builder.Services.AddEmailService(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var context =
		scope.ServiceProvider.GetRequiredService<MiniLojaVirtual.Infrastructure.Contexts.ApplicationDbContext>();
	context.Database.EnsureCreated();
	context.Database.Migrate();

	await MiniLojaVirtual.Infrastructure.Data.SeedData.SeedAsync(app.Services);
}

app.UseWebApp();

app.Run();