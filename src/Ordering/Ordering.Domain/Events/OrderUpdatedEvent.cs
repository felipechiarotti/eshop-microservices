using Ordering.Domain.Models;

public record OrderUpdatedEvent(Order order) : IDomainEvent;