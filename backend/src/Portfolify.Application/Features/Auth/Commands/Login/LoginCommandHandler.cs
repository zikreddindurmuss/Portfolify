using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.Common.Interfaces;
using Portfolify.Domain.Interfaces;
using DomainRefreshToken = Portfolify.Domain.Entities.RefreshToken;

namespace Portfolify.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly LoginCommandValidator _validator;

    public LoginCommandHandler(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPasswordHasher hasher,
        IJwtTokenGenerator tokenGenerator,
        LoginCommandValidator validator)
    {
        _users          = users;
        _refreshTokens  = refreshTokens;
        _hasher         = hasher;
        _tokenGenerator = tokenGenerator;
        _validator      = validator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // ── 1. Format validasyonu ────────────────────────────────────────────
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var message = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        // ── 2. Kullanıcı bul ─────────────────────────────────────────────────
        var user = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return Error.InvalidCredentials;

        // ── 3. Şifre doğrula ─────────────────────────────────────────────────
        if (!_hasher.Verify(request.Password, user.PasswordHash))
            return Error.InvalidCredentials;

        // ── 4. JWT access token + refresh token üret ───────────────────────────
        var (accessToken, expiresAt) = _tokenGenerator.GenerateAccessToken(
            user.Id, user.Email, user.Username, user.Slug);
        var (refreshTokenValue, refreshTokenExpiresAt) = _tokenGenerator.GenerateRefreshToken();

        // ── 5. Refresh token'ı DB'ye kaydet ─────────────────────────────────
        var refreshToken = DomainRefreshToken.Create(user.Id, refreshTokenValue, refreshTokenExpiresAt);
        await _refreshTokens.AddRefreshTokenAsync(refreshToken, cancellationToken);
        await _refreshTokens.SaveChangesAsync(cancellationToken);

        // ── 6. Sonucu döndür ─────────────────────────────────────────────────
        return new LoginResponse(
            user.Id,
            user.Email,
            user.Username,
            user.Slug,
            user.CreatedAt,
            accessToken,
            refreshTokenValue,
            expiresAt);
    }
}
