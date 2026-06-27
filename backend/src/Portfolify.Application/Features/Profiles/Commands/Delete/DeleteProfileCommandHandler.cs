using MediatR;
using Portfolify.Application.Common;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Profiles.Commands.Delete;

public sealed class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Result>
{
    private readonly IProfileRepository _profiles;

    public DeleteProfileCommandHandler(IProfileRepository profiles) => _profiles = profiles;

    public async Task<Result> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profiles.GetByIdAsync(request.Id, cancellationToken);
        if (profile is null)
            return Error.ProfileNotFound;

        await _profiles.DeleteAsync(profile, cancellationToken);
        await _profiles.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
