using Domain.Base.Interfaces;

namespace Domain.Base;

/// <summary>
/// Rastreia a criação e a modificação de objetos.
/// </summary>
public abstract class Auditable : IAuditable
{
    public const int CreatedByMaxLength = 128;
    public const int UpdatedByMaxLength = 128;

    private const string SystemUser = "System";
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public string CreatedBy
    {
        get;
        private set
        {
            ThrowIfNullOrWhiteSpace(value, nameof(CreatedBy));
            ThrowIfGreaterThan(value.Length, CreatedByMaxLength, nameof(CreatedBy));
            field = value;
        }
    }

    public string? UpdatedBy
    {
        get;
        private set
        {
            ThrowIfNullOrWhiteSpace(value, nameof(UpdatedBy));
            ThrowIfGreaterThan(value.Length, UpdatedByMaxLength, nameof(UpdatedBy));
            field = value;
        }
    }

    public void SetCreated(TimeProvider timeProvider, string? createdBy = null)
    {
        CreatedAt = timeProvider.GetUtcNow();
        CreatedBy = createdBy ?? SystemUser;
    }

    public void SetUpdated(TimeProvider timeProvider, string? updatedBy = null)
    {
        UpdatedAt = timeProvider.GetUtcNow();
        UpdatedBy = updatedBy ?? SystemUser;
    }
}