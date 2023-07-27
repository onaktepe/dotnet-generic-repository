namespace DotNetBB.Repository.EF.Config;

public class ContextScopedCommitBehavior : ITransactionalBehavior
{
    public CommitBehavior CommitBehavior => CommitBehavior.ContextScoped;
}