using Microsoft.AspNetCore.Mvc;
namespace FiveTalents.Api.Controllers;

public class AttendanceController : BaseController
{
    [HttpGet("sessions")] public IActionResult GetSessions(int organizationId) => Ok(new { message = "TODO", organizationId });
    [HttpPost("sessions")] public IActionResult CreateSession() => StatusCode(501);
    [HttpGet("sessions/{id}")] public IActionResult GetSession(int id) => Ok(new { message = "TODO", id });
    [HttpPost("sessions/{id}/records")] public IActionResult RecordAttendance(int id) => StatusCode(501);
    [HttpGet("sessions/{id}/records")] public IActionResult GetRecords(int id) => Ok(new { message = "TODO", id });
    [HttpGet("members/{memberId}/history")] public IActionResult GetMemberHistory(int memberId) => Ok(new { message = "TODO", memberId });
}
