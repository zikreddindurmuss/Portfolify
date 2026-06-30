using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolify.Application.Common;
using Portfolify.Application.Features.Auth.Commands.Login;
using Portfolify.Application.Features.Auth.Commands.Logout;
using Portfolify.Application.Features.Auth.Commands.Register;
using Portfolify.Application.Features.Auth.Commands.RefreshToken;

namespace Portfolify.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Yeni kullanıcı kaydı</summary>
    /// <remarks>
    /// Slug, username'den otomatik üretilir (küçük harf, boşluk → tire).
    /// Şifre BCrypt ile hash'lenip saklanır.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        var command = new RegisterCommand(request.Username, request.Email, request.Password);
        var result  = await _mediator.Send(command, ct);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    /// <summary>Kullanıcı girişi</summary>
    /// <remarks>
    /// Email + şifre doğrulanır, JWT access token ve refresh token döner.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result  = await _mediator.Send(command, ct);

        if (result.IsFailure)
            return MapAuthError(result.Error!);

        return Ok(ToAuthResponse(result.Value!));
    }

    /// <summary>Token yenileme</summary>
    /// <remarks>
    /// Geçerli bir refresh token karşılığında yeni access token + yeni refresh token döner.
    /// Eski refresh token rotation ile revoke edilir.
    /// </remarks>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken ct)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result  = await _mediator.Send(command, ct);

        if (result.IsFailure)
            return MapAuthError(result.Error!);

        return Ok(ToAuthResponse(result.Value!));
    }

    /// <summary>Çıkış yapma</summary>
    /// <remarks>Verilen refresh token'ı revoke eder.</remarks>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken ct)
    {
        var command = new LogoutCommand(request.RefreshToken);
        var result  = await _mediator.Send(command, ct);

        if (result.IsFailure)
            return MapAuthError(result.Error!);

        return NoContent();
    }

    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });

    // ── Yardımcılar ──────────────────────────────────────────────────────────

    /// <summary>LoginResponse'u frontend'in beklediği AuthResponse şekline çevirir.</summary>
    private static object ToAuthResponse(LoginResponse value) => new
    {
        accessToken = value.AccessToken,
        refreshToken = value.RefreshToken,
        expiresAt = value.ExpiresAt,
        user = new
        {
            id = value.UserId,
            username = value.Username,
            email = value.Email,
            slug = value.Slug,
            createdAt = value.CreatedAt
        }
    };

    /// <summary>
    /// Auth hatalarını uygun HTTP status koduna çevirir:
    /// kimlik bilgisi / refresh token geçersizse 401, diğer hatalarda 400.
    /// </summary>
    private IActionResult MapAuthError(Error error)
    {
        if (error == Error.InvalidCredentials || error == Error.InvalidRefreshToken)
            return Unauthorized(new { error });

        return BadRequest(new { error });
    }
}

// ── Request DTO ──────────────────────────────────────────────────────────────
/// <summary>Register endpoint için giriş modeli</summary>
public sealed record RegisterRequest(
    string Username,
    string Email,
    string Password
);

/// <summary>Login endpoint için giriş modeli</summary>
public sealed record LoginRequest(
    string Email,
    string Password
);

/// <summary>Refresh endpoint için giriş modeli</summary>
public sealed record RefreshTokenRequest(
    string RefreshToken
);

/// <summary>Logout endpoint için giriş modeli</summary>
public sealed record LogoutRequest(
    string RefreshToken
);
