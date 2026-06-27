using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Users.Queries.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IRequest<Result<UserDto>>;
