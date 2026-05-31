using FiveTalents.Application.Organizations.Commands;
using FiveTalents.Application.Organizations.Queries;

using Microsoft.AspNetCore.Mvc;

namespace FiveTalents.Api.Controllers;

public class OrganizationsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        => Ok(await Mediator.Send(new GetAllOrganizationsQuery(includeInactive)));

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
        => Ok(await Mediator.Send(new GetOrganizationTreeQuery()));

    [HttpGet("levels")]
    public async Task<IActionResult> GetLevels()
        => Ok(await Mediator.Send(new GetOrganizationLevelsQuery()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await Mediator.Send(new GetOrganizationByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationCommand command)
    {
        int id = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrganizationCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);
        return NoContent();
    }

    [HttpGet("{id:int}/settings")]
    public async Task<IActionResult> GetSettings(int id)
        => Ok(await Mediator.Send(new GetOrganizationSettingsQuery(id)));

    [HttpPut("{id:int}/settings")]
    public async Task<IActionResult> UpdateSettings(int id, [FromBody] UpdateOrganizationSettingsCommand command)
    {
        if (id != command.OrganizationId)
        {
            return BadRequest();
        }

        await Mediator.Send(command);
        return NoContent();
    }
}
