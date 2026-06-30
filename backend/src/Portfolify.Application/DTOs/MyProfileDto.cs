namespace Portfolify.Application.DTOs;

/// <summary>
/// "/me" profil akışı için hafif DTO — AvatarUrl bilinçli olarak dahil edilmez
/// (avatar yönetimi ileride ayrı bir OBS upload endpoint'iyle gelecek).
/// Profil henüz oluşturulmamışsa tüm alanlar null döner (404 değil).
/// </summary>
public sealed record MyProfileDto(
    string? Bio,
    string? GithubUrl,
    string? LinkedinUrl,
    string? BlogUrl
);
