using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MiniLojaVirtual.Domain.Extensions;

namespace MiniLojaVirtual.Web.Controllers.Base;

public class MainController : Controller
{
	protected readonly ILogger<MainController> Logger;

	protected MainController(ILogger<MainController> logger)
	{
		Logger = logger;
	}

	protected string? GetCurrentUserId()
	{
		return User.FindFirstValue(ClaimTypes.NameIdentifier);
	}

	protected async Task<string> SaveImage(string? imageData)
	{
		if (string.IsNullOrEmpty(imageData) ||
		    (Uri.IsWellFormedUriString(imageData, UriKind.Absolute) && !imageData.StartsWith("data:")))
			return imageData;

		try
		{
			if (imageData.StartsWith("data:image/"))
			{
				var parts = imageData.Split(',');
				var header = parts[0];
				var base64Data = parts[1];

				var fileName = GetFileName(header);
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var filePath = Path.Combine(uploadsFolder, fileName);

				var imageBytes = Convert.FromBase64String(base64Data);
				await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

				return $"/uploads/{fileName}";
			}
		}
		catch (Exception ex)
		{
			const string errorMessage = "Erro ao salvar imagem.";
			TempData["Error"] = errorMessage;
			Logger.LogError($"{errorMessage}: {ex.Message}");
		}

		return imageData;
	}

	protected static string GetFileName(string header)
	{
		var extension = ".jpg";
		if (header.Contains("image/png"))
			extension = ".png";
		else if (header.Contains("image/gif"))
			extension = ".gif";
		else if (header.Contains("image/jpeg") || header.Contains("image/jpg"))
			extension = ".jpg";

		var fileName = $"{string.Empty.NewUuidV7()}{extension}";
		return fileName;
	}
}