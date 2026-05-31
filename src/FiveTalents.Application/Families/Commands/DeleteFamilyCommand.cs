using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Families.Commands;

public record DeleteFamilyCommand(int Id) : IRequest;

public class DeleteFamilyCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteFamilyCommand>
{
    public async Task Handle(DeleteFamilyCommand request, CancellationToken cancellationToken)
    {
        var family = await db.Families
            .FirstOrDefaultAsync(f => f.Id == request.Id && !f.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Family", request.Id);

        family.IsDeleted = true;
        await db.SaveChangesAsync(cancellationToken);
    }
}
