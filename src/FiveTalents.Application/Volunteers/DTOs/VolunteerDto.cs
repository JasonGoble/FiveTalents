using FiveTalents.Domain.Volunteers;
namespace FiveTalents.Application.Volunteers.DTOs;

public record VolunteerOpportunityDto(int Id, string Name, string? Description, string? Department, bool IsActive, int AssignmentCount);
public record VolunteerAssignmentDto(int Id, int OpportunityId, string OpportunityName, int MemberId, string MemberName, DateTime ScheduledDate, AssignmentStatus Status);
