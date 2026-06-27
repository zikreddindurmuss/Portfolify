namespace Portfolify.Application.DTOs;

public sealed record ProfileDto(
    Guid Id,
    Guid UserId,
    string? Bio,
    string? AvatarUrl,
    string? GithubUrl,
    string? LinkedinUrl,
    string? BlogUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
