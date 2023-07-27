namespace DotNetBB.Repository.EF.Config;

public interface ITransactionalBehavior
{
    CommitBehavior CommitBehavior { get; }
}