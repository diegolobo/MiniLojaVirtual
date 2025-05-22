using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace MiniLojaVirtual.Application.Mvc;

public static class ErrorMessagesTranslator
{
	public static void ConfigurePortugueseErrorMessages(this DefaultModelBindingMessageProvider messageProvider)
	{
		if (messageProvider == null)
			throw new ArgumentNullException(nameof(messageProvider), "Message provider cannot be null.");

		messageProvider.SetAttemptedValueIsInvalidAccessor((value, field) =>
			$"O valor '{value}' não é válido para o campo '{field}'.");
		messageProvider.SetMissingBindRequiredValueAccessor(field => $"O campo '{field}' é obrigatório.");
		messageProvider.SetMissingKeyOrValueAccessor(() => "Uma chave ou valor está faltando.");
		messageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "O corpo da solicitação está ausente.");
		messageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(value =>
			$"O valor '{value}' não é válido.");
		messageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() =>
			"O valor fornecido não é válido.");
		messageProvider.SetUnknownValueIsInvalidAccessor(fieldName =>
			$"O valor fornecido não é válido para o campo '{fieldName}'.");
		messageProvider.SetValueIsInvalidAccessor(value =>
			$"O valor '{value}' não é válido.");
		messageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "Este campo deve ser um número");
		messageProvider.SetValueMustNotBeNullAccessor(fieldName =>
			$"O campo '{fieldName}' não pode ser nulo.");
		messageProvider.SetValueMustBeANumberAccessor(fieldName => $"O campo '{fieldName}' deve ser um número");
	}
}