using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Profiles.Commands.UpdateMyProfile;

/// <summary>
/// JWT'deki kullanıcı kimliğiyle profil upsert eder.
/// AvatarUrl bilinçli olarak buraya dahil değildir — ileride ayrı bir OBS upload akışıyla yönetilecek.
/// </summary>
public sealed record UpdateMyProfileCommand(
    Guid UserId,
    string? Bio,
    string? GithubUrl,
    string? LinkedinUrl,
    string? BlogUrl
) : IRequest<Result<MyProfileDto>>;
