using MediatR;
using Portfolify.Application.Common;
using Portfolify.Domain.Interfaces;

namespace Portfolify.Application.Features.Skills.Commands.RemoveEndorsement;

public sealed class RemoveEndorsementCommandHandler : IRequestHandler<RemoveEndorsementCommand, Result>
{
    private readonly ISkillRepository _skills;

    public RemoveEndorsementCommandHandler(ISkillRepository skills) => _skills = skills;

    public async Task<Result> Handle(RemoveEndorsementCommand request, CancellationToken cancellationToken)
    {
        var endorsement = await _skills.GetEndorsementAsync(request.SkillId, request.EndorsedByUserId, cancellationToken);
        if (endorsement is null)
            return Error.EndorsementNotFound;

        await _skills.RemoveEndorsementAsync(endorsement, cancellationToken);
        await _skills.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
