global using Microsoft.EntityFrameworkCore;
global using static System.ArgumentException;
global using static System.ArgumentNullException;
global using static System.ArgumentOutOfRangeException;
global using Ardalis.Specification;
global using FluentValidation;
global using ErrorOr;
global using Vogen;
global using WebApi.Common.Persistence;

// As diretivas `using` implícitas do HotChocolate trazem `HotChocolate.Error`, o que gera um conflito com o tipo `ErrorOr`
// retornado pela camada de domínio. Neste projeto, "Error" refere-se ao tipo de resultado da camada de domínio.
global using Error = ErrorOr.Error;
