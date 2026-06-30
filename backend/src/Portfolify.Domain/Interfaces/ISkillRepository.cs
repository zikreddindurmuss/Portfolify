using Portfolify.Domain.Entities;

namespace Portfolify.Domain.Interfaces;

public interface ISkillRepository
{
    Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Endorsement koleksiyonu yüklenmiş tek skill (sayım/isEndorsedByMe hesapları için).</summary>
    Task<Skill?> GetByIdWithEndorsementsAsync(Guid id, CancellationToken ct = default);

    /// <summary>Bir kullanıcının tüm skill'leri, endorsement koleksiyonlarıyla birlikte.</summary>
    Task<List<Skill>> GetByUserIdWithEndorsementsAsync(Guid userId, CancellationToken ct = default);

    Task<bool> ExistsByUserIdAndNameAsync(Guid userId, string name, CancellationToken ct = default);

    Task AddAsync(Skill skill, CancellationToken ct = default);
    Task DeleteAsync(Skill skill, CancellationToken ct = default);

    Task<SkillEndorsement?> GetEndorsementAsync(Guid skillId, Guid endorsedByUserId, CancellationToken ct = default);
    Task AddEndorsementAsync(SkillEndorsement endorsement, CancellationToken ct = default);
    Task RemoveEndorsementAsync(SkillEndorsement endorsement, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
