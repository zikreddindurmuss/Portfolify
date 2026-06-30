using MediatR;
using Portfolify.Application.Common;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Auth.Commands.Logout;

/// <summary>Logout: gelen refresh token'ı bulup revoke eder.</summary>
public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly LogoutCommandValidator _validator;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokens,
        LogoutCommandValidator validator)
    {
        _refreshTokens = refreshTokens;
        _validator     = validator;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
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

        // ── 3. Revoke et + kaydet ────────────────────────────────────────────
        await _refreshTokens.RevokeAsync(existingToken, cancellationToken);
        await _refreshTokens.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
