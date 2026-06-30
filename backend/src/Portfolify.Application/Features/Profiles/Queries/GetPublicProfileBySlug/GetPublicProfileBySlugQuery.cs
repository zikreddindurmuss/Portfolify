using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Profiles.Queries.GetPublicProfileBySlug;

/// <summary>Tamamen public profil sayfası (portfolify.app/{slug}) için sorgu.</summary>
public sealed record GetPublicProfileBySlugQuery(string Slug) : IRequest<Result<PublicProfileDto>>;
