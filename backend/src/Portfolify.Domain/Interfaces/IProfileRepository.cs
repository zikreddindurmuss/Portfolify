using Portfolify.Domain.Entities;

namespace Portfolify.Domain.Interfaces;

public interface IProfileRepository
{
    Task<Profile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Profile profile, CancellationToken ct = default);
    Task UpdateAsync(Profile profile, CancellationToken ct = default);
    Task DeleteAsync(Profile profile, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
