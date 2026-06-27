using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Users.Queries.GetByEmail;

public sealed class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Result<UserDto>>
{
    private readonly IUserRepository _users;

    public GetUserByEmailQueryHandler(IUserRepository users) => _users = users;

    public async Task<Result<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return Error.UserNotFound;

        return new UserDto(user.Id, user.Username, user.Email, user.Slug, user.CreatedAt);
    }
}
