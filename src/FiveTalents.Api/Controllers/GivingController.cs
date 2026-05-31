using Microsoft.AspNetCore.Mvc;
namespace FiveTalents.Api.Controllers;

public class GivingController : BaseController
{
    [HttpGet("donations")] public IActionResult GetDonations(int organizationId) => Ok(new { message = "TODO", organizationId });
    [HttpPost("donations")] public IActionResult CreateDonation() => StatusCode(501);
    [HttpGet("funds")] public IActionResult GetFunds(int organizationId) => Ok(new { message = "TODO", organizationId });
    [HttpPost("funds")] public IActionResult CreateFund() => StatusCode(501);
    [HttpGet("pledges")] public IActionResult GetPledges(int organizationId) => Ok(new { message = "TODO", organizationId });
    [HttpPost("pledges")] public IActionResult CreatePledge() => StatusCode(501);
    [HttpGet("batches")] public IActionResult GetBatches(int organizationId) => Ok(new { message = "TODO", organizationId });
    [HttpPost("batches")] public IActionResult CreateBatch() => StatusCode(501);
    [HttpGet("reports/summary")] public IActionResult GetSummary(int organizationId, DateTime from, DateTime to) => Ok(new { message = "TODO" });
}
