using FiveTalents.Application.Members.Queries;

using Microsoft.AspNetCore.Mvc;

namespace FiveTalents.Api.Controllers;

[Route("api/contact-types")]
public class ContactTypesController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
        => Ok(await Mediator.Send(new GetContactTypesQuery(), ct));
}
