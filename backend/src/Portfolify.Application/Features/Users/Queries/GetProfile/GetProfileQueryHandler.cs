using MediatR;
using Portfolify.Application.Common;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Users.Queries.GetProfile;

// TODO (Ders 3): Bu handler implement edilecek
// Adımlar:
//   1. _users.GetBySlugAsync(request.Slug) ile kullanıcı bul
//   2. Kullanıcı bulunamazsa → Error.UserNotFound döndür
//   3. User → ProfileResponse'a map et
//   4. Result<ProfileResponse>.Success(...) döndür
public sealed class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<ProfileResponse>>
{
    private readonly IUserRepository _users;

    public GetProfileQueryHandler(IUserRepository users)
    {
        _users = users;
    }

    public async Task<Result<ProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetBySlugAsync(request.Slug, cancellationToken);

        if (user is null)
            return Error.UserNotFound;

        return new ProfileResponse(
            user.Id,
            user.Username,
            user.Slug,
            user.Bio,
            user.AvatarUrl,
            user.GitHubUrl,
            user.LinkedInUrl,
            user.BlogUrl,
            user.CreatedAt
        );
    }
}
