using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace MiniLojaVirtual.Web.Controllers.Abstracts;

public abstract class MainController : Controller
{
	protected string? GetCurrentUserId()
	{
		return User.FindFirstValue(ClaimTypes.Sid);
	}
}