using DotNetBB.Repository.EF.Config;

namespace DotNetBB.Repository.EF.UoW;

public class UoW<TContext> : IUoW<TContext>
    where TContext: DbContext
{
    private DbTransaction _transaction;
    private DbContext _context;
    private DbConnection _connection;
    private bool _isConnectionCreated = false;
    private bool _isContextCreated = false;

    protected readonly IContextFactory<TContext> _contextFactory;
    protected readonly ITransactionalBehavior _transactionalBehavior;
    protected readonly ILogger<UoW<TContext>> _logger;

    public string UOWID { get; } = "";

    public UoW(IContextFactory<TContext> contextFactory, ITransactionalBehavior transactionalBehavior, ILogger<UoW<TContext>> logger)
    {
        _contextFactory = contextFactory;
        _transactionalBehavior = transactionalBehavior;
        _logger = logger;
        UOWID = Guid.NewGuid().ToString();
    }

    public TContext GetContext()
    {
        _logger.LogDebug("GetContext UOWID: " + UOWID + ", CommitBehavior:" + _transactionalBehavior.CommitBehavior + ", _isConnectionCreated:" + _isConnectionCreated + ", _isContextCreated:" + _isContextCreated);

        if (_transactionalBehavior.CommitBehavior == CommitBehavior.ContextScoped || _transactionalBehavior.CommitBehavior == CommitBehavior.ContextAuto)
        {
            _logger.LogDebug("GetContext creating context");
            _isContextCreated = true;
            _context = _contextFactory.Create(); // (DbContext)Activator.CreateInstance(_repositoryBehavior.DbContextType, connectionString);
        }
        else
        {
            if (_isContextCreated)
            {
                _logger.LogDebug("Returning existing context");
                return (TContext)_context;
            }

            _logger.LogDebug("GetContext creating context");
            _isContextCreated = true;
            _context = _contextFactory.Create(); //(DbContext)Activator.CreateInstance(_repositoryBehavior.DbContextType, _connection);

            if (!_isConnectionCreated)
            {
                _logger.LogDebug("GetContext creating connection");
                _isConnectionCreated = true;
                _connection = _context.Database.GetDbConnection(); //_repositoryBehavior.ProvidedEntityConnectionBuilder();
            }

            if (_connection.State == ConnectionState.Closed || _connection.State == System.Data.ConnectionState.Broken)
            {
                _logger.LogDebug("GetContext opening connection");
                _connection.Open();
                _transaction = _connection.BeginTransaction(IsolationLevel.Snapshot);
            }

            _context.Database.UseTransaction(_transaction);

            //context.ChangeTracker.AutoDetectChangesEnabled = false;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        }

        _logger.LogDebug("GetContext, finished");
        return (TContext)_context;
    }

    public void Commit()
    {
        _logger.LogDebug("Commit called");

        if (!_isContextCreated)
        {
            _logger.LogDebug("Commit, DbContext not created, no need to commit");
            return;
        }

        _logger.LogDebug("Commit CommitBehavior:" + _transactionalBehavior.CommitBehavior + ", _isConnectionCreated:" + _isConnectionCreated + ", _isContextCreated:" + _isContextCreated);
        if (_transactionalBehavior.CommitBehavior == CommitBehavior.DbScoped
            || _transactionalBehavior.CommitBehavior == CommitBehavior.DbAuto
            || _transactionalBehavior.CommitBehavior == CommitBehavior.DbBatch)
        {
            if (!_isConnectionCreated || _connection == null || _transaction == null) return;

            _logger.LogDebug("Commit committing transaction");
            _transaction.Commit();
        }
        else
        {
            _context.SaveChanges();
        }

        if (_transactionalBehavior.CommitBehavior == CommitBehavior.ContextScoped
            || _transactionalBehavior.CommitBehavior == CommitBehavior.DbScoped
            || _transactionalBehavior.CommitBehavior == CommitBehavior.DbBatch)
        {
            if (!_contextFactory.IsSingletonContext)
            {
                DisposeContext();
            }
            
            CloseConnection();
        }

        _logger.LogDebug("Commit finished");
        return;
    }

    public void Rollback()
    {
        _logger.LogDebug("Rollback called");
        if (!_isContextCreated)
        {
            _logger.LogDebug("Rollback, DbContext not created, no need to rollback");
            return;
        }

        _logger.LogDebug("Rollback CommitBehavior:" + _transactionalBehavior.CommitBehavior + ", _isConnectionCreated:" + _isConnectionCreated + ", _isContextCreated:" + _isContextCreated);

        if (_transactionalBehavior.CommitBehavior == CommitBehavior.DbScoped
            || _transactionalBehavior.CommitBehavior == CommitBehavior.DbAuto
            || _transactionalBehavior.CommitBehavior == CommitBehavior.DbBatch)
        {
            if (!_isConnectionCreated || _connection == null || _transaction == null) return;

            _logger.LogDebug("Rollback rollbacking transaction");
            _transaction.Rollback();
        }
        
        if (_transactionalBehavior.CommitBehavior == CommitBehavior.ContextScoped
            || _transactionalBehavior.CommitBehavior == CommitBehavior.DbScoped
            || _transactionalBehavior.CommitBehavior == CommitBehavior.DbBatch)
        {
            if (!_contextFactory.IsSingletonContext)
            {
                DisposeContext();
            }
            CloseConnection();
        }

        _logger.LogDebug("Rollback finished");
        return;
    }

    private void DisposeContext()
    {
        _logger.LogDebug("DisposeContext, _isContextCreated:" + _isContextCreated);
        if (!_isContextCreated || _context == null) return;

        _isContextCreated = false;

        try
        {
            _logger.LogDebug("DisposeContext, disposing context");
            _context.Dispose();
            _context = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DisposeContext, dispose context error:");
        }
    }

    private void CloseConnection()
    {
        _logger.LogDebug("CloseConnection, _isConnectionCreated:" + _isConnectionCreated);
        if (!_isConnectionCreated && _connection == null) return;

        _isConnectionCreated = false;

        try
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
            {
                _logger.LogDebug("CloseConnection, closing connection");
                _connection.Close();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CloseConnection, close connection error:");
        }

        try
        {
            _logger.LogDebug("CloseConnection, disposing connection");
            _connection.Dispose();
            _connection = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CloseConnection, dispose connection error:");
        }
    }
}