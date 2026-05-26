using FiveTalents.Application.Families.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FiveTalents.Api.Controllers;

[Route("api/family-roles")]
public class FamilyRolesController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(int organizationId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetFamilyRolesQuery(organizationId), ct));
}
