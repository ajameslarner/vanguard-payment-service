using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Entities.Base;

[ExcludeFromCodeCoverage]
public abstract class TEntity
{
    public Guid Id { get; set; }
    public virtual DateTimeOffset CreatedAt { get; set; }
    public virtual string LastUpdatedBy { get; set; }
    public virtual DateTimeOffset LastUpdatedOn { get; set; }
}
