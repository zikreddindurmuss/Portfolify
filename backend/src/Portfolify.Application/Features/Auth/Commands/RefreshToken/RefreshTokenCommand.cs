using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.Features.Auth.Commands.Login;

namespace Portfolify.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string RefreshToken
) : IRequest<Result<LoginResponse>>;
