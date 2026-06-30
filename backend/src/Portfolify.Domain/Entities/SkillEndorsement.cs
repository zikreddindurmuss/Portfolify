using Portfolify.Domain.Common;

namespace Portfolify.Domain.Entities;

/// <summary>
/// Bir kullanıcının başka bir kullanıcının skill'ini onaylaması (endorse).
/// Aynı kullanıcı aynı skill'i iki kez endorse edemez (SkillId + EndorsedByUserId unique constraint).
/// Kendi skill'ini endorse edemez — bu kural Application katmanında (EndorseSkillCommandHandler) uygulanır.
/// </summary>
public sealed class SkillEndorsement : Entity<Guid>
{
    public Guid SkillId { get; private set; }
    public Guid EndorsedByUserId { get; private set; }

    public Skill Skill { get; private set; } = null!;
    public User EndorsedByUser { get; private set; } = null!;

    // EF Core constructor
    private SkillEndorsement() { }

    public static SkillEndorsement Create(Guid skillId, Guid endorsedByUserId)
    {
        return new SkillEndorsement
        {
            Id = Guid.NewGuid(),
            SkillId = skillId,
            EndorsedByUserId = endorsedByUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
