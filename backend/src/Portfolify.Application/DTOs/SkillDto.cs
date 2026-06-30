namespace Portfolify.Application.DTOs;

/// <summary>
/// Bir kullanıcının skill'i, herkese açık görünüm için.
/// IsEndorsedByMe, isteğe bağlı currentUserId parametresine göre hesaplanır —
/// anonim ziyaretçi için her zaman false.
/// </summary>
public sealed record SkillDto(
    Guid Id,
    string Name,
    int EndorsementCount,
    bool IsEndorsedByMe
);
