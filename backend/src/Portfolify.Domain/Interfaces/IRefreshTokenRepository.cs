using Portfolify.Domain.Entities;

namespace Portfolify.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task RevokeAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
