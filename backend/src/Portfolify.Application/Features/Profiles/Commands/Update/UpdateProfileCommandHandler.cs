using MediatR;
using Portfolify.Application.Common;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Profiles.Commands.Update;

public sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly IProfileRepository _profiles;

    public UpdateProfileCommandHandler(IProfileRepository profiles) => _profiles = profiles;

    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profiles.GetByIdAsync(request.Id, cancellationToken);
        if (profile is null)
            return Error.ProfileNotFound;

        profile.Update(request.Bio, request.AvatarUrl, request.GithubUrl, request.LinkedinUrl, request.BlogUrl);

        await _profiles.UpdateAsync(profile, cancellationToken);
        await _profiles.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
