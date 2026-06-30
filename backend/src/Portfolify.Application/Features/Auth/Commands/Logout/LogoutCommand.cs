using MediatR;
using Portfolify.Application.Common;

namespace Portfolify.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(
    string RefreshToken
) : IRequest<Result>;
