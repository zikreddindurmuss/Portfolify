using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.Common.Interfaces;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Auth.Commands.Login;

// TODO (Ders 3): Bu handler implement edilecek
// Adımlar:
//   1. Validator ile format kontrolü yap
//   2. Email ile kullanıcı bul (_users.GetByEmailAsync)
//   3. Kullanıcı bulunamazsa → Error.InvalidCredentials döndür
//   4. BCrypt ile şifre doğrula (_hasher.Verify)
//   5. Şifre yanlışsa → Error.InvalidCredentials döndür (kullanıcı bilgisi sızdırma)
//   6. JWT access token üret (SecretKey, Issuer, Audience, ExpiresInMinutes)
//   7. Refresh token üret (Guid.NewGuid() veya random bytes)
//   8. Result<LoginResponse>.Success(...) döndür
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly LoginCommandValidator _validator;

    public LoginCommandHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        LoginCommandValidator validator)
    {
        _users     = users;
        _hasher    = hasher;
        _validator = validator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // ── 1. Format validasyonu ────────────────────────────────────────────
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var message = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        // ── 2. Kullanıcı bul ─────────────────────────────────────────────────
        var user = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return Error.InvalidCredentials;

        // ── 3. Şifre doğrula ─────────────────────────────────────────────────
        if (!_hasher.Verify(request.Password, user.PasswordHash))
            return Error.InvalidCredentials;

        // TODO: JWT token üretimi burada yapılacak
        throw new NotImplementedException("JWT token üretimi Ders 3'te implement edilecek.");
    }
}
