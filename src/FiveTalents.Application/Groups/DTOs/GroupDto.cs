using FiveTalents.Domain.Groups;

namespace FiveTalents.Application.Groups.DTOs;

public record GroupDto(
    int Id,
    string Name,
    string? Description,
    int GroupTypeId,
    string GroupTypeName,
    string? GroupTypeColor,
    string? GroupTypeIcon,
    GroupStatus Status,
    int MemberCount,
    int? LeaderMemberId,
    string? LeaderName,
    int? CoLeaderMemberId,
    string? CoLeaderName,
    MeetingFrequency? MeetingFrequency,
    string? MeetingDay,
    string? MeetingTime,
    string? MeetingLocation,
    int? MaxCapacity,
    bool IsOpenToNewMembers,
    string? ImageUrl,
    int OrganizationId,
    string? OrgName = null
);

public record GroupTypeSummaryDto(int Id, string Name, string? Color, string? IconName);
