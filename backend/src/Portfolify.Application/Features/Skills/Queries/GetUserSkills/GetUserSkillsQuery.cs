using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Skills.Queries.GetUserSkills;

/// <summary>
/// Slug ile kullanıcının skill listesini getirir.
/// CurrentUserId opsiyoneldir — anonim ziyaretçi için null geçilir,
/// bu durumda her skill'in IsEndorsedByMe alanı false döner.
/// </summary>
public sealed record GetUserSkillsQuery(
    string Slug,
    Guid? CurrentUserId
) : IRequest<Result<List<SkillDto>>>;
