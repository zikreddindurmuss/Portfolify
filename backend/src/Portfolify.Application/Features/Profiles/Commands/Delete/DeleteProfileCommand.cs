using MediatR;
using Portfolify.Application.Common;

namespace Portfolify.Application.Features.Profiles.Commands.Delete;

public sealed record DeleteProfileCommand(Guid Id) : IRequest<Result>;
