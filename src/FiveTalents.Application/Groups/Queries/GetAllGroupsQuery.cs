using FiveTalents.Application.Common.Interfaces;
using FiveTalents.Application.Groups.DTOs;
using FiveTalents.Domain.Groups;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace FiveTalents.Application.Groups.Queries;

public record GetAllGroupsQuery(int OrganizationId, string? Search = null, GroupStatus? Status = null)
    : IRequest<List<GroupDto>>;

public class GetAllGroupsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetAllGroupsQuery, List<GroupDto>>
{
    public async Task<List<GroupDto>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var query = db.Groups
            .Where(g => g.OrganizationId == request.OrganizationId && !g.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(g => g.Name.Contains(request.Search) ||
                (g.Description != null && g.Description.Contains(request.Search)));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(g => g.Status == request.Status);
        }

        return await query
            .OrderBy(g => g.Name)
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
            .ToListAsync(cancellationToken);
    }
}
