using Microsoft.AspNetCore.Mvc;
using StreamLab.Lab.DataProtectionApi.Schema;
using StreamLab.Lab.DataProtectionApi.Services;

namespace StreamLab.Lab.DataProtectionApi.Controllers;

[Route("api/[controller]")]
public class MarkedStringController : MarkedDataController<MarkedString>
{
    private readonly MarkedStringService svc;

    public MarkedStringController(MarkedStringService svc) : base(svc)
    {
        this.svc = svc;
    }

    [HttpGet("[action]/{data}/{label}/{marking}")]
    public async Task<IActionResult> Create(
        [FromRoute]string data,
        [FromRoute]string label,
        [FromRoute]string marking
    ) => ApiReturn(await svc.Create(data, label, marking));

    [HttpPost("[action]")]
    public async Task<IActionResult> Decrypt([FromBody]MarkedString data) =>
        ApiReturn(await svc.Decrypt(data));
}