using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Families.Commands;

public record RemoveFamilyMemberCommand(int FamilyId, int MemberId) : IRequest;

public class RemoveFamilyMemberCommandHandler(IApplicationDbContext db)
    : IRequestHandler<RemoveFamilyMemberCommand>
{
    public async Task Handle(RemoveFamilyMemberCommand request, CancellationToken cancellationToken)
    {
        var membership = await db.FamilyMembers
            .FirstOrDefaultAsync(
                fm => fm.FamilyId == request.FamilyId && fm.MemberId == request.MemberId,
                cancellationToken)
            ?? throw new NotFoundException("FamilyMember", $"{request.FamilyId}/{request.MemberId}");

        db.FamilyMembers.Remove(membership);
        await db.SaveChangesAsync(cancellationToken);
    }
}
