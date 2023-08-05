using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TodosController : ControllerBase
{

    private readonly ITodoService _todoService;
    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }


    [HttpGet]
    public async Task<List<TodoSummaryModel>> GetTodos()
    {
       var result = await _todoService.GetTodosAsync();
       return result;
    }

    [HttpGet("{id}")]
    public async Task<TodoModel> GetTodo(int id)
    {
       var result = await _todoService.GetTodoAsync(id);
       return result;
    }

    [HttpPut("{id}")]
    public async Task UpdateTodo(int id, UpdateTodoModel request)
    {
       await _todoService.UpdateTodoAsync(id, request);
       return;
    }

    [HttpPost]
    public async Task CreateTodo(CreateTodoModel request)
    {
       await _todoService.CreateTodoAsync(request);
       return;
    }

    [HttpDelete("{id}")]
    public async Task DeleteTodo(int id)
    {
       await _todoService.DeleteTodoAsync(id);
       return;
    }
}
