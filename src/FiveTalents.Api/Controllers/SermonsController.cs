using Microsoft.AspNetCore.Mvc;
namespace FiveTalents.Api.Controllers;

public class SermonsController : BaseController
{
    [HttpGet] public IActionResult GetAll(int organizationId, int page = 1, int pageSize = 12) => Ok(new { message = "TODO", organizationId });
    [HttpGet("{id}")] public IActionResult GetById(int id) => Ok(new { message = "TODO", id });
    [HttpPost] public IActionResult Create() => StatusCode(501);
    [HttpPut("{id}")] public IActionResult Update(int id) => StatusCode(501);
    [HttpDelete("{id}")] public IActionResult Delete(int id) => StatusCode(501);
    [HttpGet("series")] public IActionResult GetSeries(int organizationId) => Ok(new { message = "TODO" });
    [HttpPost("series")] public IActionResult CreateSeries() => StatusCode(501);
    [HttpPost("{id}/view")] public IActionResult IncrementView(int id) => StatusCode(501);
}
