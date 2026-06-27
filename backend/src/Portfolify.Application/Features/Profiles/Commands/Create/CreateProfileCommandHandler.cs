using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Profiles.Commands.Create;

public sealed class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, Result<ProfileDto>>
{
    private readonly IProfileRepository _profiles;
    private readonly IUserRepository _users;

    public CreateProfileCommandHandler(IProfileRepository profiles, IUserRepository users)
    {
        _profiles = profiles;
        _users = users;
    }

    public async Task<Result<ProfileDto>> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        if (await _users.GetByIdAsync(request.UserId, cancellationToken) is null)
            return Error.UserNotFound;

        if (await _profiles.ExistsByUserIdAsync(request.UserId, cancellationToken))
            return Error.ProfileAlreadyExists;

        var profile = Profile.Create(request.UserId, request.Bio, request.AvatarUrl,
            request.GithubUrl, request.LinkedinUrl, request.BlogUrl);

        await _profiles.AddAsync(profile, cancellationToken);
        await _profiles.SaveChangesAsync(cancellationToken);

        return ToDto(profile);
    }

    private static ProfileDto ToDto(Profile p) => new(p.Id, p.UserId, p.Bio, p.AvatarUrl,
        p.GithubUrl, p.LinkedinUrl, p.BlogUrl, p.CreatedAt, p.UpdatedAt);
}
