using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Profiles.Queries.GetBySlug;

public sealed class GetProfileBySlugQueryHandler : IRequestHandler<GetProfileBySlugQuery, Result<ProfileDto>>
{
    private readonly IProfileRepository _profiles;
    private readonly IUserRepository _users;

    public GetProfileBySlugQueryHandler(IProfileRepository profiles, IUserRepository users)
    {
        _profiles = profiles;
        _users = users;
    }

    public async Task<Result<ProfileDto>> Handle(GetProfileBySlugQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetBySlugAsync(request.Slug, cancellationToken);
        if (user is null)
            return Error.UserNotFound;

        var profile = await _profiles.GetByUserIdAsync(user.Id, cancellationToken);
        if (profile is null)
            return Error.ProfileNotFound;

        return ToDto(profile);
    }

    private static ProfileDto ToDto(Profile p) => new(p.Id, p.UserId, p.Bio, p.AvatarUrl,
        p.GithubUrl, p.LinkedinUrl, p.BlogUrl, p.CreatedAt, p.UpdatedAt);
}
