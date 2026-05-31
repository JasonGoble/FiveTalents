using FiveTalents.Domain.Events;
namespace FiveTalents.Application.Events.DTOs;

public record EventDto(int Id, string Title, string? Description, DateTime StartDateTime, DateTime EndDateTime, string? Location, EventStatus Status, int RegistrationCount, int OrganizationId);
public record EventSummaryDto(int Id, string Title, DateTime StartDateTime, string? Location, EventStatus Status);
