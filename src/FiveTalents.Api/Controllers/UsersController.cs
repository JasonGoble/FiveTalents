using FiveTalents.Application.Users.Queries;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiveTalents.Api.Controllers;

public class UsersController : BaseController
{
    [HttpGet("unlinked")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> GetUnlinked(CancellationToken ct = default)
        => Ok(await Mediator.Send(new GetUnlinkedUsersQuery(), ct));
}
