using Microsoft.EntityFrameworkCore;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;
using Portfolify.Infrastructure.Persistence;

namespace Portfolify.Infrastructure.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db) => _db = db;

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, ct);

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct = default)
        => await _db.RefreshTokens.AddAsync(refreshToken, ct);

    public Task RevokeAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        refreshToken.Revoke();
        _db.RefreshTokens.Update(refreshToken);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
