using DotNetBB.Repository.Abstraction.Interface;
using SampleApp.Domain;

public class TodoService : ITodoService
{
    protected readonly ICrudRepository<Todo> _todoRepository;
    
    public TodoService(ICrudRepository<Todo> todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task CreateTodoAsync(CreateTodoModel request)
    {
        var entity = new Todo
        {
            Name = request.Name ?? throw new ArgumentNullException(nameof(request.Name)),
            Priority = request.Priority,
            Status = Constants.TodoStatus.New
        };

        await _todoRepository.AddAsync(entity);

        return;

    }

    public async Task DeleteTodoAsync(int todoId)
    {
        var entity = await _todoRepository.FindSingleAsync(p => p.Id == todoId) ?? throw new Exception("TodoNotFound");
        await _todoRepository.DeleteAsync(entity);

        return;
    }

    public async Task<TodoModel> GetTodoAsync(int todoId)
    {
        var entity = await _todoRepository.FindSingleAsync(p=> p.Id == todoId) ?? throw new Exception("TodoNotFound");
        
        return new TodoModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Priority = entity.Priority,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedDate = entity.CreatedDate,
            ModifiedBy = entity.ModifiedBy,
            ModifiedDate = entity.ModifiedDate
        };
    }

    public async Task<List<TodoSummaryModel>> GetTodosAsync()
    {
        var entities = await _todoRepository.FindAllAsync();
        var result = new List<TodoSummaryModel>();
        foreach (var entity in entities)
        {
            result.Add(new TodoSummaryModel{
                Id = entity.Id,
                Name = entity.Name,
                Priority = entity.Priority,
                Status = entity.Status
            });
        }

        return result;
    }

    public async Task UpdateTodoAsync(int todoId, UpdateTodoModel request)
    {
       var entity = await _todoRepository.FindSingleAsync(p=> p.Id == todoId) ?? throw new Exception("TodoNotFound");
       entity.Priority = request.Priority;
       entity.Status = request.Status;
       
       await _todoRepository.UpdateAsync(entity);

       return;
    }
    
}