using FiveTalents.Application.Families.Commands;
using FiveTalents.Application.Families.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FiveTalents.Api.Controllers;

public class FamiliesController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(int organizationId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetFamiliesQuery(organizationId), ct));

    [HttpGet("member/{memberId}")]
    public async Task<IActionResult> GetByMember(int memberId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetMemberFamiliesQuery(memberId), ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
        => Ok(await Mediator.Send(new GetFamilyByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFamilyCommand command, CancellationToken ct)
    {
        var id = await Mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFamilyCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteFamilyCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id}/members")]
    public async Task<IActionResult> AddMember(int id, [FromBody] AddFamilyMemberRequest request, CancellationToken ct)
    {
        await Mediator.Send(new AddFamilyMemberCommand(id, request.MemberId, request.RoleId), ct);
        return NoContent();
    }

    [HttpPut("{id}/members/{memberId}/role")]
    public async Task<IActionResult> UpdateMemberRole(
        int id, int memberId, [FromBody] UpdateMemberRoleRequest request, CancellationToken ct)
    {
        await Mediator.Send(new UpdateFamilyMemberRoleCommand(id, memberId, request.RoleId), ct);
        return NoContent();
    }

    [HttpDelete("{id}/members/{memberId}")]
    public async Task<IActionResult> RemoveMember(int id, int memberId, CancellationToken ct)
    {
        await Mediator.Send(new RemoveFamilyMemberCommand(id, memberId), ct);
        return NoContent();
    }
}

public record AddFamilyMemberRequest(int MemberId, int RoleId);
public record UpdateMemberRoleRequest(int RoleId);
