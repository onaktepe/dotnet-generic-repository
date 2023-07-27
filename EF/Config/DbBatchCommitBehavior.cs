namespace DotNetBB.Repository.EF.Config;

public class DbBatchCommitBehavior : ITransactionalBehavior
{
    public CommitBehavior CommitBehavior => CommitBehavior.DbBatch;
}