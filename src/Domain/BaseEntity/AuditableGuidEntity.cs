using System.Diagnostics.CodeAnalysis;

namespace SourceName.Domain.BaseEntity;

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal")]
public abstract class AuditableGuidEntity
{
    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; internal set; }
    
    protected AuditableGuidEntity()
    {
        this.Id = Guid.NewGuid();
        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
    }
    
    protected AuditableGuidEntity(Guid id, DateTime createdAt, DateTime updatedAt)
    {
        this.Id = id;
        this.CreatedAt = createdAt;
        this.UpdatedAt = updatedAt;
    }

    protected void Update() => this.UpdatedAt = DateTime.UtcNow;

    public override bool Equals(object? obj)
    {
        return obj != null && !(this.GetType() != obj.GetType()) && this.Id == ((AuditableGuidEntity) obj).Id;
    }

    public override int GetHashCode() => this.Id.GetHashCode();
}
