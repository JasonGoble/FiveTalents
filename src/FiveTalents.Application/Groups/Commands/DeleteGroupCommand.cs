using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Groups.Commands;

public record DeleteGroupCommand(int Id) : IRequest;

public class DeleteGroupCommandHandler(IApplicationDbContext db) : IRequestHandler<DeleteGroupCommand>
{
    public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await db.Groups
            .FirstOrDefaultAsync(g => g.Id == request.Id && !g.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Group", request.Id);

        group.IsDeleted = true;
        await db.SaveChangesAsync(cancellationToken);
    }
}
