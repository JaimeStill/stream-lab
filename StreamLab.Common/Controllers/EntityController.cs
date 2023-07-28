using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamLab.Common.Schema;
using StreamLab.Common.Services;

namespace StreamLab.Common.Controllers;
public abstract class EntityController<T, Db> : ApiController
    where T : Entity
    where Db : DbContext
{
    protected readonly IService<T, Db> baseSvc;

    public EntityController(IService<T, Db> svc)
    {
        baseSvc = svc;
    }

    [HttpGet("[action]")]
    public virtual async Task<IActionResult> Get() =>
        ApiReturn(await baseSvc.Get());

    [HttpGet("[action]/{id:int}")]
    public virtual async Task<IActionResult> GetById([FromRoute]int id) =>
        ApiReturn(await baseSvc.GetById(id));
        
    [HttpPost("[action]")]
    public virtual async Task<IActionResult> Validate([FromBody]T entity) =>
        ApiReturn(await baseSvc.Validate(entity));

    [HttpPost("[action]")]
    public virtual async Task<IActionResult> Save([FromBody]T entity) =>
        ApiReturn(await baseSvc.Save(entity));

    [HttpDelete("[action]")]
    public virtual async Task<IActionResult> Remove([FromBody]T entity) =>
        ApiReturn(await baseSvc.Remove(entity));
}