using Microsoft.EntityFrameworkCore;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;
using Portfolify.Infrastructure.Persistence;

namespace Portfolify.Infrastructure.Repositories;

public sealed class SkillRepository : ISkillRepository
{
    private readonly AppDbContext _db;

    public SkillRepository(AppDbContext db) => _db = db;

    public async Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Skills.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Skill?> GetByIdWithEndorsementsAsync(Guid id, CancellationToken ct = default)
        => await _db.Skills
            .Include(s => s.Endorsements)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<List<Skill>> GetByUserIdWithEndorsementsAsync(Guid userId, CancellationToken ct = default)
        => await _db.Skills
            .Include(s => s.Endorsements)
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync(ct);

    public async Task<bool> ExistsByUserIdAndNameAsync(Guid userId, string name, CancellationToken ct = default)
        => await _db.Skills.AnyAsync(s => s.UserId == userId && s.Name == name, ct);

    public async Task AddAsync(Skill skill, CancellationToken ct = default)
        => await _db.Skills.AddAsync(skill, ct);

    public Task DeleteAsync(Skill skill, CancellationToken ct = default)
    {
        _db.Skills.Remove(skill);
        return Task.CompletedTask;
    }

    public async Task<SkillEndorsement?> GetEndorsementAsync(Guid skillId, Guid endorsedByUserId, CancellationToken ct = default)
        => await _db.SkillEndorsements
            .FirstOrDefaultAsync(e => e.SkillId == skillId && e.EndorsedByUserId == endorsedByUserId, ct);

    public async Task AddEndorsementAsync(SkillEndorsement endorsement, CancellationToken ct = default)
        => await _db.SkillEndorsements.AddAsync(endorsement, ct);

    public Task RemoveEndorsementAsync(SkillEndorsement endorsement, CancellationToken ct = default)
    {
        _db.SkillEndorsements.Remove(endorsement);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
