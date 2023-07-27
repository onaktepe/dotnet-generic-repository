namespace DotNetBB.Repository.Abstraction.Entity;

public class CrudEntity: BaseEntity
{
    public DateTime? ModifiedDate { get; set; }
    public string ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
}
