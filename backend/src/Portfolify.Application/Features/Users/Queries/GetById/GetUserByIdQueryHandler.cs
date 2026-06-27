using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Users.Queries.GetById;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _users;

    public GetUserByIdQueryHandler(IUserRepository users) => _users = users;

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return Error.UserNotFound;

        return new UserDto(user.Id, user.Username, user.Email, user.Slug, user.CreatedAt);
    }
}
