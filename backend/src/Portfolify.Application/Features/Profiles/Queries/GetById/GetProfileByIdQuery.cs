using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Profiles.Queries.GetById;

public sealed record GetProfileByIdQuery(Guid Id) : IRequest<Result<ProfileDto>>;
