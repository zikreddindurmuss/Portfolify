namespace Portfolify.Application.Common;

/// <summary>
/// Yapılandırılmış hata nesnesi.
/// Code: makine tarafından okunabilir ("User.NotFound", "Auth.InvalidCredentials")
/// Message: kullanıcıya gösterilebilir açıklama
/// </summary>
public sealed record Error(string Code, string Message)
{
    // --- Auth ---
    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "Email veya şifre hatalı.");

    public static readonly Error Unauthorized =
        new("Auth.Unauthorized", "Bu işlem için yetkiniz yok.");

    public static readonly Error InvalidRefreshToken =
        new("Auth.InvalidRefreshToken", "Refresh token geçersiz, süresi dolmuş veya iptal edilmiş.");

    // --- User ---
    public static readonly Error UserNotFound =
        new("User.NotFound", "Kullanıcı bulunamadı.");

    public static readonly Error EmailAlreadyInUse =
        new("User.EmailAlreadyInUse", "Bu e-posta adresi zaten kullanımda.");

    public static readonly Error UsernameAlreadyTaken =
        new("User.UsernameAlreadyTaken", "Bu kullanıcı adı zaten kullanımda.");

    public static readonly Error SlugAlreadyTaken =
        new("User.SlugAlreadyTaken", "Bu kullanıcı adı/slug zaten alınmış.");

    // --- Validation ---
    public static Error Validation(string message) =>
        new("Validation.Error", message);
    // --- Profile ---
    public static readonly Error ProfileNotFound =
        new("Profile.NotFound", "Profil bulunamadı.");

    public static readonly Error ProfileAlreadyExists =
        new("Profile.AlreadyExists", "Bu kullanıcının zaten bir profili var.");

    // --- Skill ---
    public static readonly Error SkillNotFound =
        new("Skill.NotFound", "Skill bulunamadı.");

    public static readonly Error SkillAlreadyExists =
        new("Skill.AlreadyExists", "Bu skill zaten eklenmiş.");

    public static readonly Error CannotEndorseOwnSkill =
        new("Skill.CannotEndorseOwnSkill", "Kendi skill'ini endorse edemezsin.");

    public static readonly Error EndorsementAlreadyExists =
        new("Skill.EndorsementAlreadyExists", "Bu skill'i zaten endorse ettin.");

    public static readonly Error EndorsementNotFound =
        new("Skill.EndorsementNotFound", "Endorsement bulunamadı.");
}
