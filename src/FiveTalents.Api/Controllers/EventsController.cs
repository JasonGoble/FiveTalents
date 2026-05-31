using Microsoft.AspNetCore.Mvc;
namespace FiveTalents.Api.Controllers;

public class EventsController : BaseController
{
    [HttpGet] public IActionResult GetAll(int organizationId) => Ok(new { message = "TODO", organizationId });
    [HttpGet("{id}")] public IActionResult GetById(int id) => Ok(new { message = "TODO", id });
    [HttpPost] public IActionResult Create() => StatusCode(501);
    [HttpPut("{id}")] public IActionResult Update(int id) => StatusCode(501);
    [HttpDelete("{id}")] public IActionResult Delete(int id) => StatusCode(501);
    [HttpGet("{id}/registrations")] public IActionResult GetRegistrations(int id) => Ok(new { message = "TODO" });
    [HttpPost("{id}/registrations")] public IActionResult Register(int id) => StatusCode(501);
    [HttpDelete("{id}/registrations/{regId}")] public IActionResult CancelRegistration(int id, int regId) => StatusCode(501);
}
