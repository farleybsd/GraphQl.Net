namespace WebApi.Features.Heroes.CreateHero;

public sealed record CreateHeroInput(
    string Name,
    string Alias,
    IReadOnlyList<CreateHeroPowerInput> Powers);
/// <remarks>
/// Utilizar um tipo de entrada específico para cada segmento, em vez de um tipo compartilhado, mantém os segmentos independentes. O custo disso é a inclusão de um segundo tipo — estruturalmente idêntico — no esquema; o benefício é que alterações no que <c>createHero</c> aceita não afetam o que <c>updateHero</c> aceita.
/// </remarks>
public sealed record CreateHeroPowerInput(string Name, int PowerLevel);


