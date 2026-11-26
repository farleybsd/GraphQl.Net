using Domain.Heroes;
using WebApi.Common.GraphQL.Erros;

namespace WebApi.Features.Heroes.CreateHero;

[MutationType]
public static partial class CreateHeroMutation
{
    [Error<InputValidationError>]
    public static async Task<Hero> CreateHeroAsync(
        CreateHeroInput input,
        IValidator<CreateHeroInput> validator,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);
        var hero =  Hero.Create(input.Name, input.Alias);
        var powers = input.Powers.Select(p => new Power(p.Name, p.PowerLevel));
        hero.UpdatePowers(powers);

        dbContext.Heroes.Add(hero);
        await dbContext.SaveChangesAsync(cancellationToken);

        return hero;
    }
}
