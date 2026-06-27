using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.Common.Interfaces;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Users.Commands.Create;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public CreateUserCommandHandler(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _users.ExistsByEmailAsync(request.Email, cancellationToken))
            return Error.EmailAlreadyInUse;

        if (await _users.GetByUsernameAsync(request.Username, cancellationToken) is not null)
            return Error.UsernameAlreadyTaken;

        var slug = request.Username.Trim().ToLowerInvariant().Replace(" ", "-");
        if (await _users.ExistsBySlugAsync(slug, cancellationToken))
            return Error.SlugAlreadyTaken;

        var user = User.Create(request.Email, request.Username, slug, _hasher.Hash(request.Password));
        await _users.AddAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        return new UserDto(user.Id, user.Username, user.Email, user.Slug, user.CreatedAt);
    }
}
