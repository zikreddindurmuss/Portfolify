using Microsoft.EntityFrameworkCore;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;
using Portfolify.Infrastructure.Persistence;

namespace Portfolify.Infrastructure.Repositories;

public sealed class ProfileRepository : IProfileRepository
{
    private readonly AppDbContext _db;

    public ProfileRepository(AppDbContext db) => _db = db;

    public async Task<Profile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Profiles.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _db.Profiles.AnyAsync(p => p.UserId == userId, ct);

    public async Task AddAsync(Profile profile, CancellationToken ct = default)
        => await _db.Profiles.AddAsync(profile, ct);

    public Task UpdateAsync(Profile profile, CancellationToken ct = default)
    {
        _db.Profiles.Update(profile);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Profile profile, CancellationToken ct = default)
    {
        _db.Profiles.Remove(profile);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
