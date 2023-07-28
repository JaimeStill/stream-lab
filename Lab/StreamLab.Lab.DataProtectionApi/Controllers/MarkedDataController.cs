using Microsoft.AspNetCore.Mvc;
using StreamLab.Common.Controllers;
using StreamLab.Lab.DataProtectionApi.Schema;
using StreamLab.Lab.DataProtectionApi.Services;

namespace StreamLab.Lab.DataProtectionApi.Controllers;

public abstract class MarkedDataController<T> : ApiController
where T : MarkedData
{
    protected readonly MarkedDataService<T> baseSvc;

    public MarkedDataController(MarkedDataService<T> svc)
    {
        baseSvc = svc;
    }

    [HttpGet("[action]/{markId:int}")]
    public async Task<IActionResult> Get([FromRoute]int markId) =>
        ApiReturn(await baseSvc.Get(markId));

    [HttpGet("[action]/{id:int}")]
    public async Task<IActionResult> GetById([FromRoute]int id) =>
        ApiReturn(await baseSvc.GetById(id));

    [HttpPost("[action]")]
    public async Task<IActionResult> Remove([FromBody]T data) =>
        ApiReturn(await baseSvc.Remove(data));
}