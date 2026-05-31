using Microsoft.AspNetCore.Mvc;
namespace FiveTalents.Api.Controllers;

public class CommunicationController : BaseController
{
    [HttpGet("templates")] public IActionResult GetTemplates(int organizationId) => Ok(new { message = "TODO", organizationId });
    [HttpPost("templates")] public IActionResult CreateTemplate() => StatusCode(501);
    [HttpPost("send/email")] public IActionResult SendEmail() => StatusCode(501);
    [HttpPost("send/sms")] public IActionResult SendSms() => StatusCode(501);
    [HttpGet("logs")] public IActionResult GetLogs(int organizationId) => Ok(new { message = "TODO" });
}
