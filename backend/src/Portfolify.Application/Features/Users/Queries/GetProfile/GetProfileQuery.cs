using MediatR;
using Portfolify.Application.Common;

namespace Portfolify.Application.Features.Users.Queries.GetProfile;

/// <summary>
/// Public kullanıcı profili — slug ile sorgulama.
/// portfolify.app/{slug} adresinde gösterilecek veriler.
/// </summary>
public sealed record GetProfileQuery(string Slug) : IRequest<Result<ProfileResponse>>;

public sealed record ProfileResponse(
    Guid UserId,
    string Username,
    string Slug,
    string? Bio,
    string? AvatarUrl,
    string? GitHubUrl,
    string? LinkedInUrl,
    string? BlogUrl,
    DateTime CreatedAt
);
