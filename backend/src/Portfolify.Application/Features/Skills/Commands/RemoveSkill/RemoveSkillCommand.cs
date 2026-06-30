using MediatR;
using Portfolify.Application.Common;

namespace Portfolify.Application.Features.Skills.Commands.RemoveSkill;

/// <summary>JWT'deki kullanıcı kimliğiyle kendi skill'ini siler.</summary>
public sealed record RemoveSkillCommand(
    Guid UserId,
    Guid SkillId
) : IRequest<Result>;
