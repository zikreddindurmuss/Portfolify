using Portfolify.Domain.Common;

namespace Portfolify.Domain.Entities;

/// <summary>
/// Refresh token — access token süresi dolduğunda yenisini almak için kullanılır.
/// Stateful: DB'de saklanır, revoke edilebilir (logout veya rotation sırasında).
/// </summary>
public sealed class RefreshToken : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }

    public User User { get; private set; } = null!;

    // EF Core constructor
    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>Token süresi dolmamış ve revoke edilmemişse geçerlidir.</summary>
    public bool IsValid() => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    public void Revoke()
    {
        IsRevoked = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
