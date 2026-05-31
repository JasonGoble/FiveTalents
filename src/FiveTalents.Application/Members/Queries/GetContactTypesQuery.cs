using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Members.DTOs;
using FiveTalents.Domain.Members;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Members.Queries;

public record GetContactTypesQuery : IRequest<ContactTypesDto>;

public class GetContactTypesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetContactTypesQuery, ContactTypesDto>
{
    public async Task<ContactTypesDto> Handle(GetContactTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await db.ContactTypes
            .Where(t => t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(cancellationToken);

        return new ContactTypesDto(
            AddressTypes: types.Where(t => t.Category == ContactTypeCategory.Address)
                               .Select(t => new ContactTypeDto(t.Id, t.Name)).ToList(),
            EmailTypes: types.Where(t => t.Category == ContactTypeCategory.Email)
                               .Select(t => new ContactTypeDto(t.Id, t.Name)).ToList(),
            PhoneTypes: types.Where(t => t.Category == ContactTypeCategory.Phone)
                               .Select(t => new ContactTypeDto(t.Id, t.Name)).ToList()
        );
    }
}
