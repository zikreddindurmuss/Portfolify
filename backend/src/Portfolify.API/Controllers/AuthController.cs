using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolify.Application.Features.Auth.Commands.Register;

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
    // POST api/auth/login
    // TODO (Ders 3): implement edilecek

    /// <summary>Token yenileme</summary>
    // POST api/auth/refresh
    // TODO (Ders 3): implement edilecek

    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });
}

// ── Request DTO ──────────────────────────────────────────────────────────────
/// <summary>Register endpoint için giriş modeli</summary>
public sealed record RegisterRequest(
    string Username,
    string Email,
    string Password
);
