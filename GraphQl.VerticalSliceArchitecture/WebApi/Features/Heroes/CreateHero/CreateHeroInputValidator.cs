using Domain.Heroes;

namespace WebApi.Features.Heroes.CreateHero;

public class CreateHeroInputValidator : AbstractValidator<CreateHeroInput>
{
    public CreateHeroInputValidator()
    {
        RuleFor(v => v.Name)
           .NotEmpty()
           .MaximumLength(Hero.NameMaxLength);

        RuleFor(v => v.Alias)
            .NotEmpty()
            .MaximumLength(Hero.AliasMaxLength);

        // O RuleForEach ignora silenciosamente uma coleção nula, e o resolvedor enumera
        // "powers" incondicionalmente; portanto, sem isso, uma requisição que omita "powers" causa uma falha no resolvedor.
        RuleFor(v => v.Powers)
            .NotNull();

        RuleForEach(v => v.Powers)
            .ChildRules(power =>
            {
                power.RuleFor(p => p.Name)
                    .NotEmpty()
                    .MaximumLength(Power.NameMaxLength);

                power.RuleFor(p => p.PowerLevel)
                    .InclusiveBetween(1, 10);
            });
    }
}
