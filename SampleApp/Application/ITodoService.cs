public interface ITodoService
{
    Task<List<TodoSummaryModel>> GetTodosAsync();
    Task<TodoModel> GetTodoAsync(int todoId);
    Task CreateTodoAsync(CreateTodoModel request);
    Task UpdateTodoAsync(int todoId, UpdateTodoModel request);
    Task DeleteTodoAsync(int todoId);
}