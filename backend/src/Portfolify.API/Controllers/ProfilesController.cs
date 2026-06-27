using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolify.Application.Features.Profiles.Commands.Create;
using Portfolify.Application.Features.Profiles.Commands.Delete;
using Portfolify.Application.Features.Profiles.Commands.Update;
using Portfolify.Application.Features.Profiles.Queries.GetById;
using Portfolify.Application.Features.Profiles.Queries.GetBySlug;
using Portfolify.Application.Features.Profiles.Queries.GetByUserId;

namespace Portfolify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfilesController(IMediator mediator) => _mediator = mediator;

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
}

public sealed record UpdateProfileRequest(
    string? Bio,
    string? AvatarUrl,
    string? GithubUrl,
    string? LinkedinUrl,
    string? BlogUrl
);
