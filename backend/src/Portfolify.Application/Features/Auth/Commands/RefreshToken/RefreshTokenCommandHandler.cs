using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.Common.Interfaces;
using Portfolify.Application.Features.Auth.Commands.Login;
using Portfolify.Domain.Interfaces;
using DomainRefreshToken = Portfolify.Domain.Entities.RefreshToken;

namespace Portfolify.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Refresh token rotation: gelen token doğrulanır, eskisi revoke edilir,
/// yeni access token + yeni refresh token üretilip DB'ye kaydedilir.
/// </summary>
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUserRepository _users;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly RefreshTokenCommandValidator _validator;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokens,
        IUserRepository users,
        IJwtTokenGenerator tokenGenerator,
        RefreshTokenCommandValidator validator)
    {
        _refreshTokens  = refreshTokens;
        _users          = users;
        _tokenGenerator = tokenGenerator;
        _validator      = validator;
    }

    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // ── 1. Format validasyonu ────────────────────────────────────────────
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var message = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        // ── 2. Refresh token'ı DB'de bul ─────────────────────────────────────
        var existingToken = await _refreshTokens.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existingToken is null)
            return Error.InvalidRefreshToken;

        // ── 3. Geçerlilik kontrolü (süresi dolmuş / revoke edilmiş) ──────────
        if (!existingToken.IsValid())
            return Error.InvalidRefreshToken;

        // ── 4. Kullanıcıyı bul ───────────────────────────────────────────────
        var user = await _users.GetByIdAsync(existingToken.UserId, cancellationToken);
        if (user is null)
            return Error.InvalidRefreshToken;

        // ── 5. Eski token'ı revoke et ─────────────────────────────────────────
        await _refreshTokens.RevokeAsync(existingToken, cancellationToken);

        // ── 6. Yeni access token + yeni refresh token üret ───────────────────
        var (accessToken, expiresAt) = _tokenGenerator.GenerateAccessToken(
            user.Id, user.Email, user.Username, user.Slug);
        var (newRefreshTokenValue, newRefreshTokenExpiresAt) = _tokenGenerator.GenerateRefreshToken();

        // ── 7. Yeni refresh token'ı DB'ye kaydet ─────────────────────────────
        var newRefreshToken = DomainRefreshToken.Create(user.Id, newRefreshTokenValue, newRefreshTokenExpiresAt);
        await _refreshTokens.AddRefreshTokenAsync(newRefreshToken, cancellationToken);
        await _refreshTokens.SaveChangesAsync(cancellationToken);

        // ── 8. Sonucu LoginResponse ile aynı şekilde döndür ──────────────────
        return new LoginResponse(
            user.Id,
            user.Email,
            user.Username,
            user.Slug,
            user.CreatedAt,
            accessToken,
            newRefreshTokenValue,
            expiresAt);
    }
}
