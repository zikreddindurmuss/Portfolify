using MediatR;
using Portfolify.Application.Common;
using Portfolify.Application.DTOs;

namespace Portfolify.Application.Features.Skills.Commands.AddSkill;

/// <summary>JWT'deki kullanıcı kimliğiyle kendi profiline yeni bir skill ekler.</summary>
public sealed record AddSkillCommand(
    Guid UserId,
    string Name
) : IRequest<Result<SkillDto>>;
