using FiveTalents.Application.Common.Exceptions;
using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Groups.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Groups.Queries;

public record GetGroupByIdQuery(int Id) : IRequest<GroupDto>;

public class GetGroupByIdQueryHandler(IApplicationDbContext db) : IRequestHandler<GetGroupByIdQuery, GroupDto>
{
    public async Task<GroupDto> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await db.Groups
            .Where(g => g.Id == request.Id && !g.IsDeleted)
            .Select(g => new GroupDto(
                g.Id,
                g.Name,
                g.Description,
                g.GroupTypeId,
                g.GroupType.Name,
                g.GroupType.Color,
                g.GroupType.IconName,
                g.Status,
                g.Members.Count(m => m.IsActive),
                g.LeaderMemberId,
                g.Leader != null ? g.Leader.FirstName + " " + g.Leader.LastName : null,
                g.CoLeaderMemberId,
                g.CoLeader != null ? g.CoLeader.FirstName + " " + g.CoLeader.LastName : null,
                g.MeetingFrequency,
                g.MeetingDay,
                g.MeetingTime.HasValue ? g.MeetingTime.Value.ToString("HH:mm") : null,
                g.MeetingLocation,
                g.MaxCapacity,
                g.IsOpenToNewMembers,
                g.ImageUrl,
                g.OrganizationId
            ))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Group", request.Id);

        return group;
    }
}
