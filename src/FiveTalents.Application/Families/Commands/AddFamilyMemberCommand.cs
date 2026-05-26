using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Domain.Families;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Families.Commands;

public record AddFamilyMemberCommand(int FamilyId, int MemberId, int RoleId) : IRequest;

public class AddFamilyMemberCommandHandler(IApplicationDbContext db)
    : IRequestHandler<AddFamilyMemberCommand>
{
    public async Task Handle(AddFamilyMemberCommand request, CancellationToken cancellationToken)
    {
        var family = await db.Families
            .FirstOrDefaultAsync(f => f.Id == request.FamilyId && !f.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Family", request.FamilyId);

        var alreadyMember = await db.FamilyMembers
            .AnyAsync(fm => fm.FamilyId == request.FamilyId && fm.MemberId == request.MemberId,
                cancellationToken);

        if (alreadyMember)
            throw new InvalidOperationException("Member is already in this family.");

        db.FamilyMembers.Add(new FamilyMember
        {
            FamilyId = family.Id,
            MemberId = request.MemberId,
            FamilyRoleId = request.RoleId,
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
