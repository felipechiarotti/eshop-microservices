namespace Ordering.Domain.Abstractions;
public abstract class Entity<T> : IEntity
{
    public T Id { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}
