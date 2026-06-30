using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Profiles.Commands.UpdateMyProfile;

/// <summary>
/// Upsert: profil yoksa oluşturur, varsa günceller. AvatarUrl'e dokunmaz —
/// profil zaten varsa mevcut AvatarUrl'i korur, yeni oluşturuluyorsa null bırakır.
/// </summary>
public sealed class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, Result<MyProfileDto>>
{
    private readonly IProfileRepository _profiles;

    public UpdateMyProfileCommandHandler(IProfileRepository profiles) => _profiles = profiles;

    public async Task<Result<MyProfileDto>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profiles.GetByUserIdAsync(request.UserId, cancellationToken);

        if (profile is null)
        {
            // ── Profil yok → oluştur (AvatarUrl bu akışta yönetilmiyor, null) ────
            profile = Profile.Create(request.UserId, request.Bio, null,
                request.GithubUrl, request.LinkedinUrl, request.BlogUrl);

            await _profiles.AddAsync(profile, cancellationToken);
        }
        else
        {
            // ── Profil var → güncelle, mevcut AvatarUrl korunur ──────────────────
            profile.Update(request.Bio, profile.AvatarUrl,
                request.GithubUrl, request.LinkedinUrl, request.BlogUrl);

            await _profiles.UpdateAsync(profile, cancellationToken);
        }

        await _profiles.SaveChangesAsync(cancellationToken);

        return new MyProfileDto(profile.Bio, profile.GithubUrl, profile.LinkedinUrl, profile.BlogUrl);
    }
}
