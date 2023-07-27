using DotNetBB.Repository.EF.Config;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DotNetBB.Repository.EF.Repository;

public class CrudRepository<TEntity, TContext> : Repository<TEntity, TContext>, ICrudRepository<TEntity>
    where TEntity : CrudEntity
    where TContext: DbContext
{
    public CrudRepository(IUoW<TContext> uow, IUserContext userContext, ITransactionalBehavior transactionalBehavior, ILogger<CrudRepository<TEntity, TContext>> logger) 
        : base(uow, userContext, transactionalBehavior, logger)
    {
    }

    public void Update(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentException("Cannot update a null entity.");
        }

        PreAction();
        UpdateEntity(entity);
        PostAction();
    }

    public void Update(Expression<Func<TEntity, bool>> predicate, TEntity entity)
    {
        if (predicate == null)
        {
            throw new ArgumentException("Cannot delete entities, null predicate expression");
        }

        PreAction();
        _dbSet.Where(predicate).ExecuteUpdate(s => SetUpdateProperties(s, entity)
            .SetProperty(p=> p.ModifiedDate, q=> DateTime.UtcNow)
            .SetProperty(p=> p.ModifiedBy, q=> _userContext.Username)
        );
        PostAction();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentException("Cannot update a null entity.");
        }

        PreAction();
        UpdateEntity(entity);
        await PostActionAsync();
    }

    public async Task UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity)
    {
        if (predicate == null)
        {
            throw new ArgumentException("Cannot delete entities, null predicate expression");
        }

        PreAction();
        await _dbSet.Where(predicate).ExecuteUpdateAsync(s => SetUpdateProperties(s, entity)
            .SetProperty(p=> p.ModifiedDate, q=> DateTime.UtcNow)
            .SetProperty(p=> p.ModifiedBy, q=> _userContext.Username)
        );
        await PostActionAsync();
    }

    public void Delete(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentException("Cannot delete a null entity.");
        }

        PreAction();

        entity.IsDeleted = true;
        entity.DeletedDate = DateTime.UtcNow;
        _dbSet.Update(entity);

        PostAction();
    }

    public void Delete(Expression<Func<TEntity, bool>> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentException("Cannot delete entities, null predicate expression");
        }

        PreAction();

        _dbSet.Where(predicate).ExecuteUpdate(
            e => e
            .SetProperty(p=> p.IsDeleted, q=> true)
            .SetProperty(p=> p.DeletedDate, q=> DateTime.UtcNow)
        );

        PostAction();
    }

    public void DeleteAll()
    {
        PreAction();

        _dbSet.ExecuteUpdate(
            e => e
            .SetProperty(p=> p.IsDeleted, q=> true)
            .SetProperty(p=> p.DeletedDate, q=> DateTime.UtcNow)
        );

        PostAction();
    }


    public async Task DeleteAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentException("Cannot delete a null entity.");
        }

        PreAction();

        entity.IsDeleted = true;
        entity.DeletedDate = DateTime.UtcNow;
        _dbSet.Update(entity);

        await PostActionAsync();
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentException("Cannot delete entities, null predicate expression");
        }

        PreAction();

        await _dbSet.Where(predicate).ExecuteUpdateAsync(
            e => e
            .SetProperty(p=> p.IsDeleted, q=> true)
            .SetProperty(p=> p.DeletedDate, q=> DateTime.UtcNow)
        );

        await PostActionAsync();
    }

    public async Task DeleteAllAsync()
    {
        PreAction();

        await _dbSet.ExecuteUpdateAsync(
            e => e
            .SetProperty(p=> p.IsDeleted, q=> true)
            .SetProperty(p=> p.DeletedDate, q=> DateTime.UtcNow)
        );

        await PostActionAsync();
    }

    public void HardDelete(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentException("Cannot delete a null entity.");
        }

        PreAction();
        _dbSet.Remove(entity);
        PostAction();
    }

    public void HardDelete(Expression<Func<TEntity, bool>> predicate)
    {
       if (predicate == null)
        {
            throw new ArgumentException("Cannot delete entities, null predicate expression");
        }

        PreAction();
        _dbSet.Where(predicate).ExecuteDelete();
        PostAction();
    }

    public void HardDeleteAll()
    {
        PreAction();
        _dbSet.ExecuteDelete();
        PostAction();
    }

    public async Task HardDeleteAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentException("Cannot delete a null entity.");
        }

        PreAction();
        _dbSet.Remove(entity);
        await PostActionAsync();
    }

    public async Task HardDeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentException("Cannot delete entities, null predicate expression");
        }

        PreAction();
        await _dbSet.Where(predicate).ExecuteDeleteAsync();
        await PostActionAsync();
    }

    public async Task HardDeleteAllAsync()
    {
        PreAction();
        await _dbSet.ExecuteDeleteAsync();
        await PostActionAsync();
    }

     private void UpdateEntity(TEntity entity)
    {
        entity.ModifiedDate = DateTime.UtcNow;
        entity.ModifiedBy = _userContext.Username;
        var entry = _context.Entry<TEntity>(entity);

        if (entry.State == EntityState.Detached)
        {
            var pkey = entity.Id;
            TEntity attachedEntity = _dbSet.Find(entity.Id);
            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntity.Id = pkey;
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                entry.State = EntityState.Modified;
            }
        }
    }

    private SetPropertyCalls<TEntity> SetUpdateProperties(SetPropertyCalls<TEntity> props, TEntity entity)
    {   
        var entityType = entity.GetType();
        foreach (var property in entityType.GetProperties())
        {
            if(property.Name == "Id" || property.Name == "CreatedDate" || property.Name == "CreatedBy") continue;

            var val = property.GetValue(entity);
            props.SetProperty(p=> property, q=> val);
        }

        return props;
    }
    
}