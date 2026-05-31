using FiveTalents.Application.Members.Commands;
using FiveTalents.Application.Members.Queries;
using FiveTalents.Domain.Members;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiveTalents.Api.Controllers;

public class MembersController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        int? organizationId = null, int page = 1, int pageSize = 25,
        string? search = null, MemberStatus? status = null,
        bool includeChildOrgs = false,
        CancellationToken ct = default)
        => Ok(await Mediator.Send(new GetMembersQuery(organizationId, page, pageSize, search, status, includeChildOrgs), ct));

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct = default)
        => Ok(await Mediator.Send(new GetMyProfileQuery(), ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        => Ok(await Mediator.Send(new GetMemberByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMemberCommand command, CancellationToken ct = default)
    {
        int id = await Mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMemberCommand command, CancellationToken ct = default)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        await Mediator.Send(new DeleteMemberCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id}/link-user")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> LinkUser(int id, [FromBody] LinkUserRequest request, CancellationToken ct = default)
    {
        await Mediator.Send(new LinkMemberToUserCommand(id, request.UserId), ct);
        return NoContent();
    }

    [HttpDelete("{id}/link-user")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> UnlinkUser(int id, CancellationToken ct = default)
    {
        await Mediator.Send(new UnlinkMemberFromUserCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id}/invite")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> Invite(int id, [FromBody] InviteRequest request, CancellationToken ct = default)
    {
        await Mediator.Send(new InviteMemberCommand(id, request.AcceptBaseUrl), ct);
        return NoContent();
    }

    [HttpPut("{id}/organization")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> MoveOrganization(int id, [FromBody] MoveOrgRequest request, CancellationToken ct = default)
    {
        await Mediator.Send(new MoveMemberToOrganizationCommand(id, request.OrganizationId), ct);
        return NoContent();
    }
}

public record LinkUserRequest(string UserId);
public record InviteRequest(string AcceptBaseUrl);
public record MoveOrgRequest(int OrganizationId);
