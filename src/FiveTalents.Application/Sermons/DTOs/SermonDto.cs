using FiveTalents.Domain.Sermons;
namespace FiveTalents.Application.Sermons.DTOs;

public record SermonDto(int Id, string Title, string? Description, DateTime SermonDate, string? SpeakerName, string? ScriptureReference, string? SeriesTitle, VideoProvider? VideoProvider, string? VideoUrl, bool IsPublished, int ViewCount);
public record SermonSeriesDto(int Id, string Title, string? Description, DateTime? StartDate, DateTime? EndDate, int SermonCount);
