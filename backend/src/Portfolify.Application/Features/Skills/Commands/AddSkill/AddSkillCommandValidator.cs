using FluentValidation;

namespace Portfolify.Application.Features.Skills.Commands.AddSkill;

public sealed class AddSkillCommandValidator : AbstractValidator<AddSkillCommand>
{
    public AddSkillCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Skill adı boş olamaz.")
            .MaximumLength(50).WithMessage("Skill adı en fazla 50 karakter olabilir.");
    }
}
