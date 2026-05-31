using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Domain.Groups;

using MediatR;

namespace FiveTalents.Application.Groups.Commands;

public record CreateGroupCommand(
    int OrganizationId,
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
) : IRequest<int>;

public class CreateGroupCommandHandler(IApplicationDbContext db) : IRequestHandler<CreateGroupCommand, int>
{
    public async Task<int> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        Group group = new Group
        {
            OrganizationId = request.OrganizationId,
            Name = request.Name,
            Description = request.Description,
            GroupTypeId = request.GroupTypeId,
            Status = request.Status,
            LeaderMemberId = request.LeaderMemberId,
            CoLeaderMemberId = request.CoLeaderMemberId,
            MeetingFrequency = request.MeetingFrequency,
            MeetingDay = request.MeetingDay,
            MeetingTime = ParseTime(request.MeetingTime),
            MeetingLocation = request.MeetingLocation,
            MaxCapacity = request.MaxCapacity,
            IsOpenToNewMembers = request.IsOpenToNewMembers,
            ImageUrl = request.ImageUrl,
        };

        db.Groups.Add(group);
        await db.SaveChangesAsync(cancellationToken);
        return group.Id;
    }

    private static TimeOnly? ParseTime(string? time) =>
        TimeOnly.TryParse(time, out var t) ? t : null;
}
