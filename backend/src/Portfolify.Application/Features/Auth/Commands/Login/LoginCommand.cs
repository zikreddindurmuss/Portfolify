using MediatR;
using Portfolify.Application.Common;

namespace Portfolify.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>;

public sealed record LoginResponse(
    Guid UserId,
    string Email,
    string Username,
    string Slug,
    DateTime CreatedAt,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
