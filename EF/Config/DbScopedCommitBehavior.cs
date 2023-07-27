namespace DotNetBB.Repository.EF.Config;

public class DbScopedCommitBehavior : ITransactionalBehavior
{
    public CommitBehavior CommitBehavior => CommitBehavior.DbScoped;
}