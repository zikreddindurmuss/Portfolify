using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolify.Application.Common;
using Portfolify.Application.Features.Skills.Commands.AddSkill;
using Portfolify.Application.Features.Skills.Commands.EndorseSkill;
using Portfolify.Application.Features.Skills.Commands.RemoveEndorsement;
using Portfolify.Application.Features.Skills.Commands.RemoveSkill;
using Portfolify.Application.Features.Skills.Queries.GetUserSkills;

namespace Portfolify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SkillsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SkillsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Slug'a göre kullanıcının skill listesi — herkese açık</summary>
    /// <remarks>
    /// Giriş yapmış bir kullanıcı isteği yaparsa (geçerli JWT gönderilirse),
    /// her skill için IsEndorsedByMe alanı buna göre hesaplanır. Anonim ziyaretçi için her zaman false.
    /// </remarks>
    [HttpGet("{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserSkills(string slug, CancellationToken ct)
    {
        var currentUserId = TryGetUserId(out var id) ? id : (Guid?)null;
        var result = await _mediator.Send(new GetUserSkillsQuery(slug, currentUserId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>Giriş yapmış kullanıcının kendi profiline yeni bir skill ekler</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddSkill([FromBody] AddSkillRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new AddSkillCommand(userId, request.Name), ct);
        if (result.IsFailure)
            return MapError(result.Error!);

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    /// <summary>Giriş yapmış kullanıcının kendi skill'ini siler</summary>
    [HttpDelete("{skillId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSkill(Guid skillId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new RemoveSkillCommand(userId, skillId), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }

    /// <summary>Giriş yapmış kullanıcı bir skill'i endorse eder</summary>
    [HttpPost("{skillId:guid}/endorse")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EndorseSkill(Guid skillId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new EndorseSkillCommand(skillId, userId), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }

    /// <summary>Giriş yapmış kullanıcı kendi endorse'unu geri alır</summary>
    [HttpDelete("{skillId:guid}/endorse")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveEndorsement(Guid skillId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new RemoveEndorsementCommand(skillId, userId), ct);
        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }

    // ── Yardımcılar ──────────────────────────────────────────────────────────

    /// <summary>JWT'deki NameIdentifier claim'inden giriş yapmış kullanıcının id'sini okur.</summary>
    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }

    /// <summary>
    /// Hataları uygun HTTP status koduna çevirir:
    /// NotFound → 404, AlreadyExists/Conflict → 409, diğerleri → 400.
    /// </summary>
    private IActionResult MapError(Error error)
    {
        if (error == Error.SkillNotFound || error == Error.EndorsementNotFound)
            return NotFound(new { error });

        if (error == Error.SkillAlreadyExists || error == Error.EndorsementAlreadyExists)
            return Conflict(new { error });

        return BadRequest(new { error });
    }
}

// ── Request DTO ──────────────────────────────────────────────────────────────

/// <summary>Skill ekleme endpoint'i için giriş modeli</summary>
public sealed record AddSkillRequest(string Name);
