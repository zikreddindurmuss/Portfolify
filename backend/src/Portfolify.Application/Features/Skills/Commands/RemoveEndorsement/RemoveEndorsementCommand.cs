using MediatR;
using Portfolify.Application.Common;

namespace Portfolify.Application.Features.Skills.Commands.RemoveEndorsement;

/// <summary>JWT'deki kullanıcı kimliğiyle kendi endorse'unu geri alır.</summary>
public sealed record RemoveEndorsementCommand(
    Guid SkillId,
    Guid EndorsedByUserId
) : IRequest<Result>;
