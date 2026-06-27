using MediatR;
using Portfolify.Application.Common;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Users.Commands.Update;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUserRepository _users;

    public UpdateUserCommandHandler(IUserRepository users) => _users = users;

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return Error.UserNotFound;

        user.UpdateProfile(request.Bio, request.AvatarUrl, request.GitHubUrl, request.LinkedInUrl, request.BlogUrl);

        await _users.UpdateAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
