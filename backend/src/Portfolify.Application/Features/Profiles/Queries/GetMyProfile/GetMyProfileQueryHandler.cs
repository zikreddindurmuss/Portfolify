using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Profiles.Queries.GetMyProfile;

/// <summary>
/// Profil henüz oluşturulmamışsa hata değil, boş alanlı bir DTO döner —
/// ilk girişte kullanıcı profilini henüz doldurmamış olabilir.
/// </summary>
public sealed class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<MyProfileDto>>
{
    private readonly IProfileRepository _profiles;

    public GetMyProfileQueryHandler(IProfileRepository profiles) => _profiles = profiles;

    public async Task<Result<MyProfileDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _profiles.GetByUserIdAsync(request.UserId, cancellationToken);

        if (profile is null)
            return new MyProfileDto(null, null, null, null);

        return new MyProfileDto(profile.Bio, profile.GithubUrl, profile.LinkedinUrl, profile.BlogUrl);
    }
}
