using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Globalization;

namespace MiniLojaVirtual.Web.Mvc;

public class DecimalModelBinder : IModelBinder
{
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		if (bindingContext == null)
			throw new ArgumentNullException(nameof(bindingContext));

		var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
		if (valueProviderResult == ValueProviderResult.None)
			return Task.CompletedTask;

		bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

		var value = valueProviderResult.FirstValue;
		if (string.IsNullOrEmpty(value))
			return Task.CompletedTask;

		value = value.Replace(".", "").Replace(",", ".");

		if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
		{
			bindingContext.Result = ModelBindingResult.Success(parsedValue);
			return Task.CompletedTask;
		}

		bindingContext.ModelState.TryAddModelError(
			bindingContext.ModelName,
			"O valor informado não é válido para um campo decimal.");

		return Task.CompletedTask;
	}
}