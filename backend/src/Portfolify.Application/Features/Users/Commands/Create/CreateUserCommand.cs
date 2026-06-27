using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Users.Commands.Create;

public sealed record CreateUserCommand(
    string Username,
    string Email,
    string Password
) : IRequest<Result<UserDto>>;
