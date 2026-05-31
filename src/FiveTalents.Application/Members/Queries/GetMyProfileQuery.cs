using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Members.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Members.Queries;

public record GetMyProfileQuery : IRequest<MemberDto?>;

public class GetMyProfileQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser
) : IRequestHandler<GetMyProfileQuery, MemberDto?>
{
    public async Task<MemberDto?> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId == null)
        {
            return null;
        }

        var member = await db.Members
            .Include(m => m.Addresses).ThenInclude(a => a.ContactType)
            .Include(m => m.Emails).ThenInclude(e => e.ContactType)
            .Include(m => m.Phones).ThenInclude(p => p.ContactType)
            .Where(m => m.UserId == currentUser.UserId && !m.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (member == null)
        {
            return null;
        }

        string? orgName = await db.Organizations
            .Where(o => o.Id == member.OrganizationId)
            .Select(o => o.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return MemberMappings.ToDto(member, orgName);
    }
}
