using DotNetBB.Repository.EF.Config;
namespace DotNetBB.Repository.EF.Repository;

public class Repository<TEntity, TContext> : IRepository<TEntity>
    where TEntity : BaseEntity
    where TContext: DbContext
{
    protected readonly IUoW<TContext> _uow;
    protected readonly IUserContext _userContext;
    protected readonly ITransactionalBehavior _transactionalBehavior;
    protected readonly ILogger<Repository<TEntity, TContext>> _logger;

    protected DbContext _context;
    protected DbSet<TEntity> _dbSet;

    public Repository(IUoW<TContext> uow, IUserContext userContext, ITransactionalBehavior transactionalBehavior, ILogger<Repository<TEntity, TContext>> logger)
    {
        _uow = uow;
        _userContext = userContext;
        _transactionalBehavior = transactionalBehavior;
    }

    public void Add(TEntity entity)
    {
        PreAction();

        entity.CreatedDate = DateTime.UtcNow;
        entity.CreatedBy = _userContext.Username;
        _dbSet.Add(entity);
        PostAction();
    }

    public async Task AddAsync(TEntity entity)
    {
        PreAction();
        entity.CreatedDate = DateTime.UtcNow;
        entity.CreatedBy = _userContext.Username;
        _dbSet.Add(entity);
        await PostActionAsync();
    }

    public List<TEntity> FindAll()
    {
        PreAction();
        return _dbSet.AsNoTracking().ToList();
    }

    public async Task<List<TEntity>> FindAllAsync()
    {
        PreAction();
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public List<TEntity> FindAllPaged(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageIndex, int pageSize)
    {
        int skipCount = 0;
        if (pageIndex > 0)
        {
            skipCount = (pageIndex - 1) * pageSize;
        }

        PreAction();
        return orderBy(_dbSet.AsNoTracking()).Skip(skipCount).Take(pageSize).ToList();
    }

    public async Task<List<TEntity>> FindAllPagedAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageIndex, int pageSize)
    {
        int skipCount = 0;
        if (pageIndex > 0)
        {
            skipCount = (pageIndex - 1) * pageSize;
        }

        PreAction();
        return await orderBy(_dbSet.AsNoTracking()).Skip(skipCount).Take(pageSize).ToListAsync();
    }

    public List<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
    {
        PreAction();
        return _dbSet.AsNoTracking().Where(predicate).ToList();
    }

    public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        PreAction();
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public List<TEntity> FindPaged(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageIndex, int pageSize)
    {
        int skipCount = 0;
        if (pageIndex > 0)
        {
            skipCount = (pageIndex - 1) * pageSize;
        }

        PreAction();
        return orderBy(_dbSet.AsNoTracking().Where(predicate)).Skip(skipCount).Take(pageSize).ToList();
    }

    public async Task<List<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int pageIndex, int pageSize)
    {
        int skipCount = 0;
        if (pageIndex > 0)
        {
            skipCount = (pageIndex - 1) * pageSize;
        }

        PreAction();
        return await orderBy(_dbSet.AsNoTracking().Where(predicate)).Skip(skipCount).Take(pageSize).ToListAsync();
    }

    public TEntity FindSingle(Expression<Func<TEntity, bool>> predicate)
    {
        PreAction();
        return _dbSet.AsNoTracking().Where(predicate).SingleOrDefault();
    }

    public async Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>> predicate)
    {
        PreAction();
        return await _dbSet.AsNoTracking().Where(predicate).SingleOrDefaultAsync();
    }

    public int Count(Expression<Func<TEntity, bool>> predicate = null)
    {
        PreAction();
        if (predicate == null)
        {
            return _dbSet.AsNoTracking().Count();
        }
        else
        {
            return _dbSet.AsNoTracking().Count(predicate);
        }
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
    {
        PreAction();
        if (predicate == null)
        {
            return await _dbSet.AsNoTracking().CountAsync();
        }
        else
        {
            return await _dbSet.AsNoTracking().CountAsync(predicate);
        }
    }

    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
    {
        PreAction();
        return _dbSet.AsNoTracking().Where(predicate);
    }

    public IQueryable<TEntity> Query()
    {
        PreAction();
        return _dbSet.AsNoTracking();
    }

    protected void PreAction()
    {
        _context = _uow.GetContext();
        _dbSet = _context.Set<TEntity>();
        if (_dbSet == null)
        {
            throw new ArgumentException("Cannot crate dbset");
        }
    }

    protected void PostAction()
    {
        if (_transactionalBehavior.CommitBehavior != CommitBehavior.ContextScoped)
        {
            _context.SaveChanges();
        }

        if (_transactionalBehavior.CommitBehavior == CommitBehavior.ContextAuto || _transactionalBehavior.CommitBehavior == CommitBehavior.DbAuto)
        {
            _uow.Commit();
        }
    }

    protected async Task PostActionAsync()
    {
        if (_transactionalBehavior.CommitBehavior != CommitBehavior.ContextScoped)
        {
            await _context.SaveChangesAsync();
        }

        if (_transactionalBehavior.CommitBehavior == CommitBehavior.ContextAuto || _transactionalBehavior.CommitBehavior == CommitBehavior.DbAuto)
        {
            _uow.Commit();
        }
    }

}

public static class SortExtension
{
    public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>
        (this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            bool descending)
    {
        return descending ? source.OrderByDescending(keySelector)
                            : source.OrderBy(keySelector);
    }


    public static IOrderedQueryable<TSource> OrderByWithDirection<TSource, TKey>
        (this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            bool descending)
    {
        return descending ? source.OrderByDescending(keySelector)
                            : source.OrderBy(keySelector);
    }

}
