using Domain.Base.EventualConsistency;
using Domain.Base.Interfaces;

namespace Domain.Heroes;

public record PowerLevelUpdatedEvent(Hero Hero) : IDomainEvent
{
    public static readonly Error TeamNotFound = EventualConsistencyError.From(
        code: "PowerLeveUpdated.EquipeNãoEncontrada\r\n",
        description: "Equipe não encontrada");
}