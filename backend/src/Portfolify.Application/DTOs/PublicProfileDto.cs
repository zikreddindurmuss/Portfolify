namespace Portfolify.Application.DTOs;

/// <summary>
/// Tamamen public profil sayfası (portfolify.app/{slug}) için DTO.
/// Email, PasswordHash gibi hassas alanlar kesinlikle dışarı çıkmaz.
/// AvatarUrl de bilinçli olarak dahil değil (bu feature'a dahil değil).
/// </summary>
public sealed record PublicProfileDto(
    string Username,
    string Slug,
    string? Bio,
    string? GithubUrl,
    string? LinkedinUrl,
    string? BlogUrl
);
