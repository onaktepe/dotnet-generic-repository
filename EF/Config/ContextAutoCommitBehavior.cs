namespace DotNetBB.Repository.EF.Config;

public class ContextAutoCommitBehavior : ITransactionalBehavior
{
    public CommitBehavior CommitBehavior => CommitBehavior.DbAuto;
}