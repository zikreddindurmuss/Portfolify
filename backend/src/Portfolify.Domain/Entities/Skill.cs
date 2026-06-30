using Portfolify.Domain.Common;

namespace Portfolify.Domain.Entities;

/// <summary>
/// Bir kullanıcının profiline eklediği yetenek/skill.
/// Aynı kullanıcı aynı skill'i iki kez ekleyemez (UserId + Name unique constraint, bkz. AppDbContext).
/// </summary>
public sealed class Skill : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public User User { get; private set; } = null!;

    private readonly List<SkillEndorsement> _endorsements = [];
    public IReadOnlyCollection<SkillEndorsement> Endorsements => _endorsements.AsReadOnly();

    // EF Core constructor
    private Skill() { }

    public static Skill Create(Guid userId, string name)
    {
        return new Skill
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
