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

app.UseWebApp();

app.Run();