using Portfolify.Domain.Common;
using Portfolify.Domain.Events;

namespace Portfolify.Domain.Entities;

/// <summary>
/// Her kullanıcı kendi profilinin sahibidir.
/// Slug, public URL'de benzersiz kimlik görevi görür: portfolify.app/{slug}
///
/// Gelecek özellikler (Faz 2+):
///   - Followers / Following
///   - SkillEndorsements
///   - Posts / Articles
/// </summary>
public sealed class User : Entity<Guid>
{
    public string Email { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;

    /// <summary>Public profil URL'i: portfolify.app/john-doe</summary>
    public string Slug { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }

    // Social links — nullable, user fills in over time
    public string? GitHubUrl { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public string? BlogUrl { get; private set; }

    // EF Core constructor
    private User() { }

    public static User Create(
        string email,
        string username,
        string slug,
        string passwordHash)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            Username = username,
            Slug = slug.ToLowerInvariant(),
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // TODO: user.RaiseDomainEvent(new UserRegisteredEvent(user.Id));
        return user;
    }

    public void UpdateProfile(string? bio, string? avatarUrl, string? gitHubUrl, string? linkedInUrl, string? blogUrl)
    {
        Bio = bio;
        AvatarUrl = avatarUrl;
        GitHubUrl = gitHubUrl;
        LinkedInUrl = linkedInUrl;
        BlogUrl = blogUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }
}
