using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Families.Commands;

public record UpdateFamilyMemberRoleCommand(int FamilyId, int MemberId, int RoleId) : IRequest;

public class UpdateFamilyMemberRoleCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateFamilyMemberRoleCommand>
{
    public async Task Handle(UpdateFamilyMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var membership = await db.FamilyMembers
            .FirstOrDefaultAsync(
                fm => fm.FamilyId == request.FamilyId && fm.MemberId == request.MemberId,
                cancellationToken)
            ?? throw new NotFoundException("FamilyMember", $"{request.FamilyId}/{request.MemberId}");

        membership.FamilyRoleId = request.RoleId;
        await db.SaveChangesAsync(cancellationToken);
    }
}
