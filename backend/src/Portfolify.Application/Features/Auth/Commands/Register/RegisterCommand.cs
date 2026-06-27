using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Username,
    string Email,
    string Password
) : IRequest<Result<UserDto>>;
