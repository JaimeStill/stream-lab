using Microsoft.AspNetCore.Mvc;
using StreamLab.Common.Controllers;
using StreamLab.Lab.DataProtectionApi.Data;
using StreamLab.Lab.DataProtectionApi.Schema;
using StreamLab.Lab.DataProtectionApi.Services;

[Route("api/[controller]")]
public class MarkingController : EntityController<Marking, MarkingContext>
{
    private readonly MarkingService svc;

    public MarkingController(MarkingService svc) : base(svc)
    {
        this.svc = svc;
    }

    [HttpGet("[action]/{marking}")]
    public async Task<IActionResult> GetByValue([FromRoute]string marking) =>
        ApiReturn(await svc.GetByValue(marking));

    [HttpGet("[action]/{marking}")]
    public async Task<IActionResult> GetToken([FromRoute]string marking) =>
        ApiReturn(await svc.GetToken(marking));

    [HttpGet("[action]/{marking}/{payload}")]
    public async Task<IActionResult> Protect(
        [FromRoute]string marking,
        [FromRoute]string payload
    ) => ApiReturn(await svc.Protect(marking, payload));

    [HttpGet("[action]/{marking}/{payload}")]
    public async Task<IActionResult> Unprotect(
        [FromRoute]string marking,
        [FromRoute]string payload
    ) => ApiReturn(await svc.Unprotect(marking, payload));
}