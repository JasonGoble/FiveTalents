using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Families.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Families.Queries;

public record GetFamilyByIdQuery(int Id) : IRequest<FamilyDto>;

public class GetFamilyByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetFamilyByIdQuery, FamilyDto>
{
    public async Task<FamilyDto> Handle(GetFamilyByIdQuery request, CancellationToken cancellationToken)
    {
        var family = await db.Families
            .Where(f => f.Id == request.Id && !f.IsDeleted)
            .Select(f => new FamilyDto(
                f.Id,
                f.Name,
                f.OrganizationId,
                f.Members
                    .OrderBy(fm => fm.Role.SortOrder)
                    .Select(fm => new FamilyMemberDto(
                        fm.MemberId,
                        fm.Member.FirstName + " " + fm.Member.LastName,
                        fm.Member.ProfilePhotoUrl,
                        fm.FamilyRoleId,
                        fm.Role.Name,
                        fm.Role.IsAdult))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Family", request.Id);

        return family;
    }
}
