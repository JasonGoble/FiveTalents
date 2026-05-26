using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Families.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Families.Queries;

public record GetMemberFamiliesQuery(int MemberId) : IRequest<IReadOnlyList<MemberFamilyDto>>;

public class GetMemberFamiliesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetMemberFamiliesQuery, IReadOnlyList<MemberFamilyDto>>
{
    public async Task<IReadOnlyList<MemberFamilyDto>> Handle(
        GetMemberFamiliesQuery request, CancellationToken cancellationToken)
    {
        return await db.FamilyMembers
            .Where(fm => fm.MemberId == request.MemberId && !fm.Family.IsDeleted)
            .Select(fm => new MemberFamilyDto(
                fm.FamilyId,
                fm.Family.Name,
                fm.FamilyRoleId,
                fm.Role.Name,
                fm.Role.IsAdult))
            .ToListAsync(cancellationToken);
    }
}
