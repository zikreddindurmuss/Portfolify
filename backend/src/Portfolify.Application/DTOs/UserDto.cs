namespace Portfolify.Application.DTOs;

public sealed record UserDto(
    Guid Id,
    string Username,
    string Email,
    string Slug,
    DateTime CreatedAt
);
