using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Domain.Groups;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Groups.Commands;

public record UpdateGroupCommand(
    int Id,
    string Name,
    string? Description,
    int GroupTypeId,
    GroupStatus Status,
    int? LeaderMemberId,
    int? CoLeaderMemberId,
    MeetingFrequency? MeetingFrequency,
    string? MeetingDay,
    string? MeetingTime,
    string? MeetingLocation,
    int? MaxCapacity,
    bool IsOpenToNewMembers,
    string? ImageUrl
) : IRequest;

public class UpdateGroupCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateGroupCommand>
{
    public async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await db.Groups
            .FirstOrDefaultAsync(g => g.Id == request.Id && !g.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Group", request.Id);

        group.Name = request.Name;
        group.Description = request.Description;
        group.GroupTypeId = request.GroupTypeId;
        group.Status = request.Status;
        group.LeaderMemberId = request.LeaderMemberId;
        group.CoLeaderMemberId = request.CoLeaderMemberId;
        group.MeetingFrequency = request.MeetingFrequency;
        group.MeetingDay = request.MeetingDay;
        group.MeetingTime = TimeOnly.TryParse(request.MeetingTime, out var t) ? t : null;
        group.MeetingLocation = request.MeetingLocation;
        group.MaxCapacity = request.MaxCapacity;
        group.IsOpenToNewMembers = request.IsOpenToNewMembers;
        group.ImageUrl = request.ImageUrl;

        await db.SaveChangesAsync(cancellationToken);
    }
}
