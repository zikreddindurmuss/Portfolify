using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Users.Queries.GetById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;
