using Microsoft.AspNetCore.Mvc;
namespace FiveTalents.Api.Controllers;

public class VolunteersController : BaseController
{
    [HttpGet("opportunities")] public IActionResult GetOpportunities(int organizationId) => Ok(new { message = "TODO", organizationId });
    [HttpPost("opportunities")] public IActionResult CreateOpportunity() => StatusCode(501);
    [HttpGet("assignments")] public IActionResult GetAssignments(int organizationId, DateTime? date = null) => Ok(new { message = "TODO" });
    [HttpPost("assignments")] public IActionResult CreateAssignment() => StatusCode(501);
    [HttpPut("assignments/{id}/status")] public IActionResult UpdateStatus(int id) => StatusCode(501);
    [HttpGet("members/{memberId}/schedule")] public IActionResult GetMemberSchedule(int memberId) => Ok(new { message = "TODO" });
}
