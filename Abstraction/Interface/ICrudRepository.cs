using DotNetBB.Repository.Abstraction.Entity;

namespace DotNetBB.Repository.Abstraction.Interface;

public interface ICrudRepository<TEntity>: IRepository<TEntity>
    where TEntity: CrudEntity
{
    void Update(TEntity TEntity);
    void Update(Expression<Func<TEntity, bool>> predicate, TEntity entity);
    Task UpdateAsync(TEntity TEntity);
    Task UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity);
    void Delete(TEntity TEntity);
    void Delete(Expression<Func<TEntity, bool>> predicate);
    void DeleteAll();
    Task DeleteAsync(TEntity TEntity);
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    Task DeleteAllAsync();
    void HardDelete(TEntity TEntity);
    void HardDelete(Expression<Func<TEntity, bool>> predicate);
    void HardDeleteAll();
    Task HardDeleteAsync(TEntity TEntity);
    Task HardDeleteAsync(Expression<Func<TEntity, bool>> predicate);
    Task HardDeleteAllAsync();
}
