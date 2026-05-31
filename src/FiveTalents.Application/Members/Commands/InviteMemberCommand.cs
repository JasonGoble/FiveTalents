using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FiveTalents.Application.Members.Commands;

public record InviteMemberCommand(int MemberId, string AcceptBaseUrl) : IRequest;

public class InviteMemberCommandHandler(
    IApplicationDbContext db,
    IUserLinkingService userService,
    IEmailService emailService,
    ILogger<InviteMemberCommandHandler> logger
) : IRequestHandler<InviteMemberCommand>
{
    public async Task Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await db.Members
            .Include(m => m.Emails)
            .FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken)
            ?? throw new NotFoundException("Member", request.MemberId);

        string? primaryEmail = member.Emails.FirstOrDefault(e => e.IsPrimary)?.Email
                        ?? member.Emails.OrderBy(e => e.Id).FirstOrDefault()?.Email;

        if (string.IsNullOrWhiteSpace(primaryEmail))
        {
            throw new InvalidOperationException("Member does not have an email address. Add an email before sending an invite.");
        }

        if (member.UserId != null)
        {
            throw new InvalidOperationException("Member already has a linked user account.");
        }

        var existingUser = await userService.FindByEmailAsync(primaryEmail);
        string userId;

        if (existingUser != null)
        {
            if (existingUser.MemberId != null && existingUser.MemberId != member.Id)
            {
                throw new InvalidOperationException($"A user with email '{primaryEmail}' is already linked to a different member.");
            }

            userId = existingUser.Id;
            if (existingUser.MemberId == null)
            {
                await userService.SetMemberLinkAsync(userId, member.Id);
            }
        }
        else
        {
            userId = await userService.CreateUserForMemberAsync(
                primaryEmail, member.FirstName, member.LastName, member.Id, member.OrganizationId);
        }

        member.UserId = userId;
        await db.SaveChangesAsync(cancellationToken);

        string token = await userService.GenerateInviteTokenAsync(userId);
        string encodedToken = Uri.EscapeDataString(token);
        string inviteUrl = $"{request.AcceptBaseUrl.TrimEnd('/')}/auth/setup-account" +
                        $"?email={Uri.EscapeDataString(primaryEmail)}&token={encodedToken}";

        try
        {
            await emailService.SendAsync(
                to: primaryEmail,
                subject: "You've been invited to FiveTalents",
                body: $"Hello {member.FirstName},\n\nYou have been invited to access FiveTalents.\n\n" +
                      $"Click the link below to set up your account:\n\n{inviteUrl}\n\n" +
                      $"This link expires in 24 hours.",
                isHtml: false,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Invite email could not be sent to {Email}. Account was created and linked.", primaryEmail);
        }
    }
}
