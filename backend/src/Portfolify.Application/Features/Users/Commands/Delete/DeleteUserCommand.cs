using MediatR;
using Portfolify.Application.Common;

namespace Portfolify.Application.Features.Users.Commands.Delete;

public sealed record DeleteUserCommand(Guid Id) : IRequest<Result>;
