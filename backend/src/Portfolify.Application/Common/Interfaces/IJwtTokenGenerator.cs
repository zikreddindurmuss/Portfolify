namespace Portfolify.Application.Common.Interfaces;

/// <summary>JWT access token + refresh token üretimi.</summary>
public interface IJwtTokenGenerator
{
    /// <summary>Claim bazlı JWT access token üretir.</summary>
    (string AccessToken, DateTime ExpiresAt) GenerateAccessToken(
        Guid userId,
        string email,
        string username,
        string slug);

    /// <summary>Kriptografik olarak güvenli rastgele refresh token üretir (RefreshTokenExpiresInDays config'inden).</summary>
    (string RefreshToken, DateTime ExpiresAt) GenerateRefreshToken();
}
