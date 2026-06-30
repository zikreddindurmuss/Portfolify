using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Profiles.Queries.GetPublicProfileBySlug;

/// <summary>
/// Kullanıcı veya profil bulunamazsa hata döner (controller bunu 404'e çevirir).
/// Sadece public alanları (username, slug, bio, sosyal linkler) içerir —
/// email/passwordHash gibi hassas alanlar asla dönmez.
/// </summary>
public sealed class GetPublicProfileBySlugQueryHandler : IRequestHandler<GetPublicProfileBySlugQuery, Result<PublicProfileDto>>
{
    private readonly IUserRepository _users;
    private readonly IProfileRepository _profiles;

    public GetPublicProfileBySlugQueryHandler(IUserRepository users, IProfileRepository profiles)
    {
        _users = users;
        _profiles = profiles;
    }

    public async Task<Result<PublicProfileDto>> Handle(GetPublicProfileBySlugQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetBySlugAsync(request.Slug, cancellationToken);
        if (user is null)
            return Error.UserNotFound;

        var profile = await _profiles.GetByUserIdAsync(user.Id, cancellationToken);
        if (profile is null)
            return Error.ProfileNotFound;

        return new PublicProfileDto(user.Username, user.Slug, profile.Bio,
            profile.GithubUrl, profile.LinkedinUrl, profile.BlogUrl);
    }
}
