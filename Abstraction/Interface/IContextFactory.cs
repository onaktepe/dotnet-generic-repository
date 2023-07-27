namespace DotNetBB.Repository.Abstraction.Interface;

public interface IContextFactory<out TContext>
    where TContext: class
{
    public bool IsSingletonContext { get; }

    TContext Create();
}
