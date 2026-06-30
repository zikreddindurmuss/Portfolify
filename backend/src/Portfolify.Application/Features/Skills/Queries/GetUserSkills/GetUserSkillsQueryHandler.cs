using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Skills.Queries.GetUserSkills;

public sealed class GetUserSkillsQueryHandler : IRequestHandler<GetUserSkillsQuery, Result<List<SkillDto>>>
{
    private readonly IUserRepository _users;
    private readonly ISkillRepository _skills;

    public GetUserSkillsQueryHandler(IUserRepository users, ISkillRepository skills)
    {
        _users  = users;
        _skills = skills;
    }

    public async Task<Result<List<SkillDto>>> Handle(GetUserSkillsQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetBySlugAsync(request.Slug, cancellationToken);
        if (user is null)
            return Error.UserNotFound;

        var skills = await _skills.GetByUserIdWithEndorsementsAsync(user.Id, cancellationToken);

        var dtos = skills
            .Select(s => new SkillDto(
                s.Id,
                s.Name,
                EndorsementCount: s.Endorsements.Count,
                IsEndorsedByMe: request.CurrentUserId.HasValue
                    && s.Endorsements.Any(e => e.EndorsedByUserId == request.CurrentUserId.Value)))
            .ToList();

        return dtos;
    }
}
