namespace DotNetBB.Repository.Abstraction.Interface;

public interface IUoW<TContext>
{
    void Commit();
    void Rollback();
    TContext GetContext();
}