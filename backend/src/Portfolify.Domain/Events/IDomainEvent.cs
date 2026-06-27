namespace Portfolify.Domain.Events;

/// <summary>
/// Marker interface for domain events.
/// Future: UserFollowed, SkillEndorsed, ProfileViewed, etc.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
