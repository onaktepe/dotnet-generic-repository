namespace DotNetBB.Repository.Abstraction.Entity;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
}
