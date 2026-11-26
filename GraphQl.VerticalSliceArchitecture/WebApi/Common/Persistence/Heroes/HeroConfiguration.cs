using Domain.Heroes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.Common.Persistence.Heroes;

public class HeroConfiguration : AuditableConfiguration<Hero>
{
    public override void PostConfigure(EntityTypeBuilder<Hero> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Name).HasMaxLength(Hero.NameMaxLength).IsRequired();

        builder.Property(h => h.Alias).HasMaxLength(Hero.AliasMaxLength).IsRequired();

        // Conteúdo simples pode ser gravado como JSON, sem criar outra tabela.
        builder.OwnsMany(
            h => h.Powers,
            b =>
            {
                b.ToJson();
                b.Property(p => p.Name).HasMaxLength(Power.NameMaxLength).IsRequired();
            }
        );
    }
}
