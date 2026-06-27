using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolify.Application.Features.Users.Commands.Create;
using Portfolify.Application.Features.Users.Commands.Delete;
using Portfolify.Application.Features.Users.Commands.Update;
using Portfolify.Application.Features.Users.Queries.GetByEmail;
using Portfolify.Application.Features.Users.Queries.GetById;

namespace Portfolify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    /// <summary>Kullanıcıyı ID ile getir</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>Kullanıcıyı e-posta ile getir</summary>
    [HttpGet("by-email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail(string email, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserByEmailQuery(email), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>Yeni kullanıcı oluştur</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Kullanıcı profilini güncelle</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var command = new UpdateUserCommand(id, request.Bio, request.AvatarUrl,
            request.GitHubUrl, request.LinkedInUrl, request.BlogUrl);
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? NoContent() : NotFound(new { error = result.Error });
    }

    /// <summary>Kullanıcıyı sil</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteUserCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { error = result.Error });
    }
}

public sealed record UpdateUserRequest(
    string? Bio,
    string? AvatarUrl,
    string? GitHubUrl,
    string? LinkedInUrl,
    string? BlogUrl
);
