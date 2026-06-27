using MediatR;
using Portfolify.Application.Common;

namespace Portfolify.Application.Features.Users.Commands.Update;

public sealed record UpdateUserCommand(
    Guid Id,
    string? Bio,
    string? AvatarUrl,
    string? GitHubUrl,
    string? LinkedInUrl,
    string? BlogUrl
) : IRequest<Result>;
