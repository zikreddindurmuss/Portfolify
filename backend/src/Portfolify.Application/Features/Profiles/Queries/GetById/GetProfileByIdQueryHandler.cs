using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Profiles.Queries.GetById;

public sealed class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, Result<ProfileDto>>
{
    private readonly IProfileRepository _profiles;

    public GetProfileByIdQueryHandler(IProfileRepository profiles) => _profiles = profiles;

    public async Task<Result<ProfileDto>> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _profiles.GetByIdAsync(request.Id, cancellationToken);
        if (profile is null)
            return Error.ProfileNotFound;

        return ToDto(profile);
    }

    private static ProfileDto ToDto(Profile p) => new(p.Id, p.UserId, p.Bio, p.AvatarUrl,
        p.GithubUrl, p.LinkedinUrl, p.BlogUrl, p.CreatedAt, p.UpdatedAt);
}
