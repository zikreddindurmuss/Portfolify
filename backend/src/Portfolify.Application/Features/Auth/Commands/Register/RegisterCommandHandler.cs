using FluentValidation;
using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.Common.Interfaces;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        RegisterCommandValidator validator)
    {
        _users     = users;
        _hasher    = hasher;
        _validator = validator;
    }

    public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // ── 1. Format validasyonu ────────────────────────────────────────────
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var message = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        // ── 2. Unique kontroller ─────────────────────────────────────────────
        if (await _users.ExistsByEmailAsync(request.Email, cancellationToken))
            return Error.EmailAlreadyInUse;

        var existingUsername = await _users.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingUsername is not null)
            return Error.UsernameAlreadyTaken;

        // ── 3. Slug üret + unique kontrol ────────────────────────────────────
        var slug = GenerateSlug(request.Username);
        if (await _users.ExistsBySlugAsync(slug, cancellationToken))
            return Error.SlugAlreadyTaken;

        // ── 4. Şifre hash + kullanıcı oluştur ───────────────────────────────
        var passwordHash = _hasher.Hash(request.Password);
        var user = User.Create(request.Email, request.Username, slug, passwordHash);

        await _users.AddAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        // ── 5. DTO döndür ────────────────────────────────────────────────────
        return new UserDto(user.Id, user.Username, user.Email, user.Slug, user.CreatedAt);
    }

    /// <summary>username → küçük harf, boşluk → tire</summary>
    private static string GenerateSlug(string username)
        => username.Trim().ToLowerInvariant().Replace(" ", "-");
}
