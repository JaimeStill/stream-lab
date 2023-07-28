using System.Text;
using Microsoft.AspNetCore.Mvc;
using StreamLab.Common.Controllers;
using StreamLab.Lab.DataProtectionApi.Data;
using StreamLab.Lab.DataProtectionApi.Schema;
using StreamLab.Lab.DataProtectionApi.Services;

namespace StreamLab.Lab.DataProtectionApi.Controllers;

[Route("api/[controller]")]
public class MarkController : EntityController<Mark, MarkContext>
{
    private readonly MarkService svc;

    public MarkController(MarkService svc) : base(svc)
    {
        this.svc = svc;
    }

    [HttpGet("[action]/{marking}")]
    public async Task<IActionResult> GetByValue([FromRoute]string marking) =>
        ApiReturn(await svc.GetByValue(marking));
}