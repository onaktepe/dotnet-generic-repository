namespace DotNetBB.Repository.Abstraction.Interface;

public interface IQueryExecuter
{
    decimal GetDecimal(string query, object[] parameters);
    Task<decimal> GetDecimalAsync(string query, object[] parameters);
    long GetLong(string query, object[] parameters);
    Task<long> GetLongAsync(string query, object[] parameters);
    int GetInt(string query, object[] parameters);
    Task<int> GetIntAsync(string query, object[] parameters);
    short GetShort(string query, object[] parameters);
    Task<short> GetShortAsync(string query, object[] parameters);
    string GetString(string query, object[] parameters);
    Task<string> GetStringAsync(string query, object[] parameters);
    DateTime GetDateTime(string query, object[] parameters);
    Task<DateTime> GetDateTimeAsync(string query, object[] parameters);
    int ExecuteSqlCommand(string query, object[] parameters);
    Task<int> ExecuteSqlCommandAsync(string query, object[] parameters);
    List<T> GetQueryResult<T>(string query, object[] parameters) where T : class;
    Task<List<T>> GetQueryResultAsync<T>(string query, object[] parameters) where T : class;
}