using Microsoft.EntityFrameworkCore;
using StreamLab.Common.Schema;

namespace StreamLab.Common.Services;
public abstract class EntityService<T, Db> : IService<T, Db>
    where T : Entity
    where Db : DbContext
{
    protected DbSet<T> set;
    protected Func<Task<int>> SaveChanges;

    protected virtual Func<T, Task<T>>? OnAdd { get; set; }
    protected virtual Func<T, Task<T>>? OnUpdate { get; set; }
    protected virtual Func<T, Task<T>>? OnSave { get; set; }
    protected virtual Func<T, Task<T>>? OnRemove { get; set; }

    protected virtual Func<T, Task>? AfterAdd { get; set; }
    protected virtual Func<T, Task>? AfterUpdate { get; set; }
    protected virtual Func<T, Task>? AfterSave { get; set; }
    protected virtual Func<T, Task>? AfterRemove { get; set; }

    public EntityService(Db db)
    {
        set = db.Set<T>();
        SaveChanges = async () => await db.SaveChangesAsync();
    }

    #region Public

    public virtual async Task<List<T>> Get() =>
        await set.ToListAsync();

    public virtual async Task<T> GetById(int id) =>
        await set.FindAsync(id)
            ?? throw new KeyNotFoundException($"{typeof(T).Name} does not have a record with ID {id}");

    public abstract Task<ValidationResult> Validate(T entity);

    public async Task<ApiResult<T>> Save(T entity)
    {
        ValidationResult validity = await Validate(entity);

        if (validity.IsValid)
        {
            if (OnSave is not null)
                entity = await OnSave(entity);
                
            ApiResult<T> result = entity.Id > 0
                ? await Update(entity)
                : await Add(entity);

            if (AfterSave is not null)
                await AfterSave(entity);

            return result;
        }
        else
            return new(validity);
    }

    public async Task<ApiResult<int>> Remove(T entity)
    {
        try
        {
            if (OnRemove is not null)
                entity = await OnRemove(entity);

            set.Remove(entity);

            int result = await SaveChanges();

            if (result > 0)
            {
                if (AfterRemove is not null)
                    await AfterRemove(entity);

                return new(entity.Id, $"{typeof(T)} successfully removed");
            }
            else
                return new("Remove", new Exception("The operation was not successful"));
        }
        catch (Exception ex)
        {
            return new("Remove", ex);
        }
    }

    #endregion

    #region Internal

    protected async Task<ApiResult<T>> Add(T entity)
    {
        try
        {
            if (OnAdd is not null)
                entity = await OnAdd(entity);

            await set.AddAsync(entity);
            await SaveChanges();

            if (AfterAdd is not null)
                await AfterAdd(entity);

            return new(entity, $"{typeof(T)} successfully added");
        }
        catch (Exception ex)
        {
            return new("Add", ex);
        }
    }

    protected async Task<ApiResult<T>> Update(T entity)
    {
        try
        {
            if (OnUpdate is not null)
                entity = await OnUpdate(entity);

            set.Update(entity);
            await SaveChanges();

            if (AfterUpdate is not null)
                await AfterUpdate(entity);

            return new(entity, $"{typeof(T)} successfully updated");
        }
        catch (Exception ex)
        {
            return new("Update", ex);
        }
    }

    #endregion
}