using MediatR;
using Portfolify.Application.Common;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Skills.Commands.RemoveSkill;

/// <summary>
/// Skill bulunamazsa veya başka bir kullanıcıya aitse 404 döner —
/// sahiplik kontrolü kasıtlı olarak 403 yerine 404 ile gizlenir.
/// </summary>
public sealed class RemoveSkillCommandHandler : IRequestHandler<RemoveSkillCommand, Result>
{
    private readonly ISkillRepository _skills;

    public RemoveSkillCommandHandler(ISkillRepository skills) => _skills = skills;

    public async Task<Result> Handle(RemoveSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _skills.GetByIdAsync(request.SkillId, cancellationToken);
        if (skill is null || skill.UserId != request.UserId)
            return Error.SkillNotFound;

        await _skills.DeleteAsync(skill, cancellationToken);
        await _skills.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
