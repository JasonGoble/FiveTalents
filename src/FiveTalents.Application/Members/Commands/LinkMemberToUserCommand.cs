using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Members.Commands;

public record LinkMemberToUserCommand(int MemberId, string UserId) : IRequest;

public class LinkMemberToUserCommandHandler(
    IApplicationDbContext db,
    IUserLinkingService userService
) : IRequestHandler<LinkMemberToUserCommand>
{
    public async Task Handle(LinkMemberToUserCommand request, CancellationToken cancellationToken)
    {
        var member = await db.Members.FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken)
            ?? throw new NotFoundException("Member", request.MemberId);

        var user = await userService.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException("User", request.UserId);

        if (member.UserId != null && member.UserId != request.UserId)
        {
            throw new InvalidOperationException("Member is already linked to a different user account.");
        }

        if (user.MemberId.HasValue && user.MemberId != request.MemberId)
        {
            throw new InvalidOperationException("User is already linked to a different member record.");
        }

        member.UserId = user.Id;
        await db.SaveChangesAsync(cancellationToken);
        await userService.SetMemberLinkAsync(user.Id, member.Id);
    }
}
