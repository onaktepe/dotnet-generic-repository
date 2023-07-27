using DotNetBB.Repository.Abstraction.Entity;

namespace DotNetBB.Repository.Abstraction.Interface;

public interface IRepository<TEntity> 
    where TEntity : BaseEntity
{
    void Add(TEntity TEntity);
    Task AddAsync(TEntity TEntity);
    List<TEntity> FindAll();
    Task<List<TEntity>> FindAllAsync();
    List<TEntity> FindAllPaged(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageIndex, int pageSize);
    Task<List<TEntity>> FindAllPagedAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageIndex, int pageSize);
    List<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    List<TEntity> FindPaged(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageIndex, int pageSize);
    Task<List<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageIndex, int pageSize);
    TEntity FindSingle(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>> predicate);
    int Count(Expression<Func<TEntity, bool>> predicate);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> Query();
}