using Portfolify.Domain.Common;

namespace Portfolify.Domain.Entities;

public sealed class Profile : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? GithubUrl { get; private set; }
    public string? LinkedinUrl { get; private set; }
    public string? BlogUrl { get; private set; }

    public User User { get; private set; } = null!;

    private Profile() { }

    public static Profile Create(Guid userId, string? bio, string? avatarUrl, string? githubUrl, string? linkedinUrl, string? blogUrl)
    {
        return new Profile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Bio = bio,
            AvatarUrl = avatarUrl,
            GithubUrl = githubUrl,
            LinkedinUrl = linkedinUrl,
            BlogUrl = blogUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string? bio, string? avatarUrl, string? githubUrl, string? linkedinUrl, string? blogUrl)
    {
        Bio = bio;
        AvatarUrl = avatarUrl;
        GithubUrl = githubUrl;
        LinkedinUrl = linkedinUrl;
        BlogUrl = blogUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}
