using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolify.Application.Features.Profiles.Commands.Create;
using Portfolify.Application.Features.Profiles.Commands.Delete;
using Portfolify.Application.Features.Profiles.Commands.Update;
using Portfolify.Application.Features.Profiles.Commands.UpdateMyProfile;
using Portfolify.Application.Features.Profiles.Queries.GetById;
using Portfolify.Application.Features.Profiles.Queries.GetBySlug;
using Portfolify.Application.Features.Profiles.Queries.GetByUserId;
using Portfolify.Application.Features.Profiles.Queries.GetMyProfile;
using Portfolify.Application.Features.Profiles.Queries.GetPublicProfileBySlug;

namespace Portfolify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfilesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Giriş yapmış kullanıcının kendi profilini getir (JWT'deki id üzerinden)</summary>
    /// <remarks>Profil henüz oluşturulmamışsa 404 değil, boş alanlı bir profil döner.</remarks>
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetMyProfileQuery(userId), ct);
        return Ok(result.Value);
    }

    /// <summary>Giriş yapmış kullanıcının profilini günceller (yoksa oluşturur — upsert)</summary>
    /// <remarks>AvatarUrl bu endpoint'e dahil değildir, ileride OBS upload ile ayrı gelecek.</remarks>
    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var command = new UpdateMyProfileCommand(userId, request.Bio, request.GithubUrl, request.LinkedinUrl, request.BlogUrl);
        var result  = await _mediator.Send(command, ct);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>Public profil sayfası (portfolify.app/{slug}) — herkese açık</summary>
    /// <remarks>
    /// Sadece username, slug, bio ve sosyal linkleri döner — email/passwordHash gibi
    /// hassas alanlar kesinlikle dışarı çıkmaz. Kullanıcı veya profil yoksa 404.
    /// </remarks>
    [HttpGet("{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicProfile(string slug, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPublicProfileBySlugQuery(slug), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>Profili ID ile getir</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProfileByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>Profili slug ile getir (public URL: portfolify.app/{slug})</summary>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProfileBySlugQuery(slug), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>Kullanıcıya ait profili getir</summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProfileByUserIdQuery(userId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>Yeni profil oluştur</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProfileCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Profili güncelle</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var command = new UpdateProfileCommand(id, request.Bio, request.AvatarUrl,
            request.GithubUrl, request.LinkedinUrl, request.BlogUrl);
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? NoContent() : NotFound(new { error = result.Error });
    }

    /// <summary>Profili sil</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteProfileCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { error = result.Error });
    }

    // ── Yardımcılar ──────────────────────────────────────────────────────────

    /// <summary>JWT'deki NameIdentifier claim'inden giriş yapmış kullanıcının id'sini okur.</summary>
    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}

public sealed record UpdateProfileRequest(
    string? Bio,
    string? AvatarUrl,
    string? GithubUrl,
    string? LinkedinUrl,
    string? BlogUrl
);

/// <summary>"/me" profil güncelleme modeli — AvatarUrl bilinçli olarak dahil değil.</summary>
public sealed record UpdateMyProfileRequest(
    string? Bio,
    string? GithubUrl,
    string? LinkedinUrl,
    string? BlogUrl
);
