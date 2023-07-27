using DotNetBB.Repository.EF.Config;

namespace DotNetBB.Repository.EF.Repository;
public class QueryExecuter<TContext> : IQueryExecuter
    where TContext: DbContext
{
    private DbContext _context;
    private readonly IUoW<TContext> _uow;
    private readonly ITransactionalBehavior _transactionalBehavior;
    public QueryExecuter(IUoW<TContext> uow, ITransactionalBehavior transactionalBehavior)
    {
        _uow = uow;
        _transactionalBehavior = transactionalBehavior;
    }

    public decimal GetDecimal(string query, object[] parameters = null)
    {
        return ExecuteScalar<decimal>(query, parameters);
    }

    public async Task<decimal> GetDecimalAsync(string query, object[] parameters = null)
    {
        return await ExecuteScalarAsync<decimal>(query, parameters);
    }

    public long GetLong(string query, object[] parameters = null)
    {
        return ExecuteScalar<long>(query, parameters);
    }

    public async Task<long> GetLongAsync(string query, object[] parameters = null)
    {
        return await ExecuteScalarAsync<long>(query, parameters);
    }

    public int GetInt(string query, object[] parameters = null)
    {
        return ExecuteScalar<int>(query, parameters);
    }

    public async Task<int> GetIntAsync(string query, object[] parameters = null)
    {
        return await ExecuteScalarAsync<int>(query, parameters);
    }

    public short GetShort(string query, object[] parameters = null)
    {
        return ExecuteScalar<short>(query, parameters);
    }

    public async Task<short> GetShortAsync(string query, object[] parameters = null)
    {
        return await ExecuteScalarAsync<short>(query, parameters);
    }

    public string GetString(string query, object[] parameters = null)
    {
        return ExecuteScalar<string>(query, parameters);
    }

    public async Task<string> GetStringAsync(string query, object[] parameters = null)
    {
        return await ExecuteScalarAsync<string>(query, parameters);
    }

    public DateTime GetDateTime(string query, object[] parameters = null)
    {
        return ExecuteScalar<DateTime>(query, parameters);
    }

    public async Task<DateTime> GetDateTimeAsync(string query, object[] parameters = null)
    {
        return await ExecuteScalarAsync<DateTime>(query, parameters);
    }

    public int ExecuteSqlCommand(string query, object[] parameters = null)
    {
        PreAction();
        int result = 0;
        if(parameters == null)
        {
            result = _context.Database.ExecuteSqlRaw(query);
        }
        else
        {
            result = _context.Database.ExecuteSqlRaw(query, parameters);
        }
        
        PostAction();

        return result;
    }

    public async Task<int> ExecuteSqlCommandAsync(string query, object[] parameters = null)
    {
        PreAction();
        int result = 0;
        if (parameters == null)
        {
            result = await _context.Database.ExecuteSqlRawAsync(query);
        }
        else
        {
            result = await _context.Database.ExecuteSqlRawAsync(query, parameters);
        }

        PostAction();

        return result;
    }

    public List<T> GetQueryResult<T>(string query, object[] parameters)
        where T : class
    {
        PreAction();
        var result = default(List<T>);
        if (parameters == null)
        {
            result = _context.Set<T>().FromSqlRaw(query).ToList();
        }
        else
        {
            result = _context.Set<T>().FromSqlRaw(query, parameters).ToList();
        }
        
        PostAction();

        return result;
    }

    public async Task<List<T>> GetQueryResultAsync<T>(string query, object[] parameters)
        where T : class
    {
        PreAction();
        var result = default(List<T>);
        if (parameters == null)
        {
            result = await _context.Set<T>().FromSqlRaw(query).ToListAsync();
        }
        else
        {
            result = await _context.Set<T>().FromSqlRaw(query, parameters).ToListAsync();
        }

        PostAction();

        return result;
    }

    private T ExecuteScalar<T>(string query, object[] parameters)
    {
        PreAction();
        var connection = _context.Database.GetDbConnection();
        var command = connection.CreateCommand();

        if (_context.Database.CurrentTransaction != null)
        {
            command.Transaction = _context.Database.CurrentTransaction.GetDbTransaction();
        }

        command.CommandText = query;
        if (parameters != null)
        {
            foreach (var p in parameters)
            {
                command.Parameters.Add(p);
            }
        }

        var result = (T)command.ExecuteScalar();
        PostAction();

        return result;
    }

    private async Task<T> ExecuteScalarAsync<T>(string query, object[] parameters)
    {
        PreAction();
        var connection = _context.Database.GetDbConnection();
        var command = connection.CreateCommand();

        if (_context.Database.CurrentTransaction != null)
        {
            command.Transaction = _context.Database.CurrentTransaction.GetDbTransaction();
        }

        command.CommandText = query;
        if (parameters != null)
        {
            foreach (var p in parameters)
            {
                command.Parameters.Add(p);
            }
        }

        var result = (T)await command.ExecuteScalarAsync();
        PostAction();

        return result;
    }

    private void PreAction()
    {
        _context = _uow.GetContext();
    }

    private void PostAction()
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

    private void PostActionAsync()
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
}