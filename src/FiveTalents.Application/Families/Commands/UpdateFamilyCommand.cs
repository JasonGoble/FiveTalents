using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Families.Commands;

public record UpdateFamilyCommand(int Id, string Name) : IRequest;

public class UpdateFamilyCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateFamilyCommand>
{
    public async Task Handle(UpdateFamilyCommand request, CancellationToken cancellationToken)
    {
        var family = await db.Families
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Family", request.Id);

        family.Name = request.Name;
        await db.SaveChangesAsync(cancellationToken);
    }
}
