using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;
using Portfolify.Domain.Entities;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Skills.Commands.AddSkill;

public sealed class AddSkillCommandHandler : IRequestHandler<AddSkillCommand, Result<SkillDto>>
{
    private readonly ISkillRepository _skills;
    private readonly AddSkillCommandValidator _validator;

    public AddSkillCommandHandler(ISkillRepository skills, AddSkillCommandValidator validator)
    {
        _skills    = skills;
        _validator = validator;
    }

    public async Task<Result<SkillDto>> Handle(AddSkillCommand request, CancellationToken cancellationToken)
    {
        // ── 1. Format validasyonu ────────────────────────────────────────────
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var message = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
            return Error.Validation(message);
        }

        var name = request.Name.Trim();

        // ── 2. Aynı skill zaten eklenmiş mi? ─────────────────────────────────
        if (await _skills.ExistsByUserIdAndNameAsync(request.UserId, name, cancellationToken))
            return Error.SkillAlreadyExists;

        // ── 3. Oluştur ────────────────────────────────────────────────────────
        var skill = Skill.Create(request.UserId, name);

        await _skills.AddAsync(skill, cancellationToken);
        await _skills.SaveChangesAsync(cancellationToken);

        return new SkillDto(skill.Id, skill.Name, EndorsementCount: 0, IsEndorsedByMe: false);
    }
}
