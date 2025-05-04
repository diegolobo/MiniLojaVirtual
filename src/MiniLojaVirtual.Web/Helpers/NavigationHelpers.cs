using Microsoft.AspNetCore.Mvc.Rendering;

namespace MiniLojaVirtual.Web.Helpers;

public static class NavigationHelpers
{
	public static string IsActive(this IHtmlHelper htmlHelper, string controller, string? action = null)
	{
		var routeData = htmlHelper.ViewContext.RouteData;

		var routeController = routeData.Values["Controller"]?.ToString();
		var routeAction = routeData.Values["Action"]?.ToString();

		var isController = string.Equals(controller, routeController, StringComparison.OrdinalIgnoreCase);
		var isAction = action == null || string.Equals(action, routeAction, StringComparison.OrdinalIgnoreCase);

		return isController && isAction ? "active" : "";
	}

	public static string IsActivePage(this IHtmlHelper htmlHelper, string pageName)
	{
		var routePage = htmlHelper.ViewContext.RouteData.Values["Page"]?.ToString();
		return string.Equals(pageName, routePage, StringComparison.OrdinalIgnoreCase) ? "active" : "";
	}
}