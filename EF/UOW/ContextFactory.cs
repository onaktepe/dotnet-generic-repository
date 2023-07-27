namespace DotNetBB.Repository.EF.UoW;

public class ContextFactory<TContext> : IContextFactory<TContext>
    where TContext: DbContext
{
    private Action<DbContextOptionsBuilder<TContext>> _dbContextOptions;
    private Action<TContext> _contextCreated;
    private TContext _singletonDBContext;

    public bool IsSingletonContext { get; }

    public ContextFactory(Action<DbContextOptionsBuilder<TContext>> dbContextOptions, Action<TContext> contextCreated=null)
    {
        _dbContextOptions = dbContextOptions;
        _contextCreated = contextCreated;
    }

    public ContextFactory(TContext dbContext)
    {
        _singletonDBContext = dbContext;
        IsSingletonContext = true;
    }

    public TContext Create()
    {
        if (IsSingletonContext) return _singletonDBContext;

        DbContextOptionsBuilder<TContext> builder = new DbContextOptionsBuilder<TContext>();
        _dbContextOptions(builder);

        var dbContext = (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        _contextCreated?.Invoke(dbContext);

        return dbContext;
    }
}