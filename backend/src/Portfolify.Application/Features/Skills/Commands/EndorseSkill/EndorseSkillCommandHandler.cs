using MediatR;
using Portfolify.Application.Common;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Skills.Commands.EndorseSkill;

public sealed class EndorseSkillCommandHandler : IRequestHandler<EndorseSkillCommand, Result>
{
    private readonly ISkillRepository _skills;

    public EndorseSkillCommandHandler(ISkillRepository skills) => _skills = skills;

    public async Task<Result> Handle(EndorseSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _skills.GetByIdAsync(request.SkillId, cancellationToken);
        if (skill is null)
            return Error.SkillNotFound;

        // ── Kendi skill'ini endorse edemez ───────────────────────────────────
        if (skill.UserId == request.EndorsedByUserId)
            return Error.CannotEndorseOwnSkill;

        // ── Zaten endorse etmiş mi? ───────────────────────────────────────────
        var existing = await _skills.GetEndorsementAsync(skill.Id, request.EndorsedByUserId, cancellationToken);
        if (existing is not null)
            return Error.EndorsementAlreadyExists;

        var endorsement = SkillEndorsement.Create(skill.Id, request.EndorsedByUserId);

        await _skills.AddEndorsementAsync(endorsement, cancellationToken);
        await _skills.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
