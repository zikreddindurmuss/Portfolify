using MediatR;
using Portfolify.Application.Common;

namespace Portfolify.Application.Features.Profiles.Commands.Update;

public sealed record UpdateProfileCommand(
    Guid Id,
    string? Bio,
    string? AvatarUrl,
    string? GithubUrl,
    string? LinkedinUrl,
    string? BlogUrl
) : IRequest<Result>;
