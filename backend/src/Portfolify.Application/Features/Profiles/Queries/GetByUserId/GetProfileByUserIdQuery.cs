using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Profiles.Queries.GetByUserId;

public sealed record GetProfileByUserIdQuery(Guid UserId) : IRequest<Result<ProfileDto>>;
