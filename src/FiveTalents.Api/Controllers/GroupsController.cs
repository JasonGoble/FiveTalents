using FiveTalents.Application.Groups.Commands;
using FiveTalents.Application.Groups.Queries;
using FiveTalents.Domain.Groups;

using Microsoft.AspNetCore.Mvc;

namespace FiveTalents.Api.Controllers;

public class GroupsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(int organizationId, string? search, GroupStatus? status, CancellationToken ct)
        => Ok(await Mediator.Send(new GetAllGroupsQuery(organizationId, search, status), ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
        => Ok(await Mediator.Send(new GetGroupByIdQuery(id), ct));

    [HttpGet("types")]
    public async Task<IActionResult> GetTypes(int organizationId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetGroupTypesQuery(organizationId), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupCommand command, CancellationToken ct)
    {
        int id = await Mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGroupCommand command, CancellationToken ct)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteGroupCommand(id), ct);
        return NoContent();
    }
}
