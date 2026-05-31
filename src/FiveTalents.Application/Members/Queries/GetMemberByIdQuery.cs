using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Members.DTOs;
using FiveTalents.Domain.Members;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Members.Queries;

public record GetMemberByIdQuery(int Id) : IRequest<MemberDto>;

public class GetMemberByIdQueryHandler(IApplicationDbContext db) : IRequestHandler<GetMemberByIdQuery, MemberDto>
{
    public async Task<MemberDto> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var member = await db.Members
            .Include(m => m.Addresses).ThenInclude(a => a.ContactType)
            .Include(m => m.Emails).ThenInclude(e => e.ContactType)
            .Include(m => m.Phones).ThenInclude(p => p.ContactType)
            .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Member), request.Id);

        string? orgName = await db.Organizations
            .Where(o => o.Id == member.OrganizationId)
            .Select(o => o.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return MemberMappings.ToDto(member, orgName);
    }
}
