namespace WebApi.Common.GraphQL.Erros;

// <summary>Um ou mais campos de entrada falharam na validação.</summary>
/// <remarks>
/// Converte as falhas do FluentValidation em duas strings simples. Expor a
/// <c>ValidationException</c> diretamente incluiria <c>Severity</c>, <c>ErrorCode</c> e um dicionário
/// de marcadores de posição (placeholders) de mensagens no esquema público.
/// </remarks>
public sealed class InputValidationError(ValidationException exception)
{
    public string Message { get; } = "Um ou mais campos de entrada são inválidos.";

    public IReadOnlyList<InputValidationFailure> Failures { get; } = exception.Errors
                                                                            .Select(failure => new InputValidationFailure(failure.PropertyName,failure.ErrorMessage))
                                                                             .ToList();
}
public sealed record InputValidationFailure(string Field, string Message);
