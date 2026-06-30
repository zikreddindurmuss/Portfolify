using MediatR;
using Portfolify.Application.Common;

namespace Portfolify.Application.Features.Skills.Commands.EndorseSkill;

/// <summary>JWT'deki kullanıcı kimliğiyle bir skill'i endorse eder.</summary>
public sealed record EndorseSkillCommand(
    Guid SkillId,
    Guid EndorsedByUserId
) : IRequest<Result>;
