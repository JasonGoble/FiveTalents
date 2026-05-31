using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Domain.Families;

using MediatR;

namespace FiveTalents.Application.Families.Commands;

public record CreateFamilyCommand(int OrganizationId, string Name) : IRequest<int>;

public class CreateFamilyCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateFamilyCommand, int>
{
    public async Task<int> Handle(CreateFamilyCommand request, CancellationToken cancellationToken)
    {
        Family family = new Family
        {
            OrganizationId = request.OrganizationId,
            Name = request.Name,
        };

        db.Families.Add(family);
        await db.SaveChangesAsync(cancellationToken);
        return family.Id;
    }
}
