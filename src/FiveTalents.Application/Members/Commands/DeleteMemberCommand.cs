using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Domain.Members;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Members.Commands;

public record DeleteMemberCommand(int Id) : IRequest;

public class DeleteMemberCommandHandler(IApplicationDbContext db) : IRequestHandler<DeleteMemberCommand>
{
    public async Task Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await db.Members.FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Member), request.Id);

        member.IsDeleted = true;
        member.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }
}
