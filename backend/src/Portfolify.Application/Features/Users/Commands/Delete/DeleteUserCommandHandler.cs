using MediatR;
using Portfolify.Application.Common;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Users.Commands.Delete;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUserRepository _users;

    public DeleteUserCommandHandler(IUserRepository users) => _users = users;

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return Error.UserNotFound;

        await _users.DeleteAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
