using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Profiles.Queries.GetMyProfile;

public sealed record GetMyProfileQuery(
    Guid UserId
) : IRequest<Result<MyProfileDto>>;
