using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Profiles.Commands.Create;

public sealed record CreateProfileCommand(
    Guid UserId,
    string? Bio,
    string? AvatarUrl,
    string? GithubUrl,
    string? LinkedinUrl,
    string? BlogUrl
) : IRequest<Result<ProfileDto>>;
