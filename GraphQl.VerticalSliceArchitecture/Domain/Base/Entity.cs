using Domain.Base;

namespace Domain;

/// <summary>
/// Entidades possuem um ID e um ciclo de vida (ou seja, são criadas, modificadas e excluídas).
/// Elas podem ser criadas dentro do domínio, mas não externamente.
/// Elas impõem regras de negócio (ou seja, invariantes).
/// </summary>
public abstract class Entity<Tid> : Auditable
{
    public Tid Id { get; set; } = default!;
}