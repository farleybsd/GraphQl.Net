using Domain.Base.Interfaces;

namespace Domain.Base
{
    /// <summary>
    /// Agrupamento de objetos tratados como uma única unidade.
    /// Pode conter entidades, objetos de valor e outros agregados.
    /// Impõe regras de negócio (ou seja, invariantes).
    /// Pode ser criado externamente.
    /// Pode disparar eventos de domínio.
    /// Representa um limite transacional (ou seja, todas as alterações são salvas ou nenhuma é salva).
    /// </summary>
    public abstract class AggregateRoot<Tid> : Entity<Tid>, IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        public IReadOnlyList<IDomainEvent> PopDomainEvents()
        {
            var copy = _domainEvents.ToList().AsReadOnly();
            _domainEvents.Clear();
            return copy;
        }
    }
}