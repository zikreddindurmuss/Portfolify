using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Profiles.Queries.GetBySlug;

public sealed record GetProfileBySlugQuery(string Slug) : IRequest<Result<ProfileDto>>;
