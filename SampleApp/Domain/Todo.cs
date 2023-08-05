using DotNetBB.Repository.Abstraction.Entity;

namespace SampleApp.Domain;

public class Todo: CrudEntity
{
    public string Name { get; set; }
    public int Priority { get; set; }
    public string Status { get; set; }
}