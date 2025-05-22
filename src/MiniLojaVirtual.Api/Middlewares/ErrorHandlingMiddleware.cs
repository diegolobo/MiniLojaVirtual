using System.Net;
using System.Text.Json;

namespace MiniLojaVirtual.Api.Middlewares;

public class ErrorHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ErrorHandlingMiddleware> _logger;

	public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro não tratado");
			await HandleExceptionAsync(context);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context)
	{
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

		var response = new
		{
			statusCode = context.Response.StatusCode,
			message = "Ocorreu um erro interno no servidor."
		};

		return context.Response.WriteAsync(JsonSerializer.Serialize(response));
	}
}