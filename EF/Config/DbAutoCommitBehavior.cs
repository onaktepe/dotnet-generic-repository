namespace DotNetBB.Repository.EF.Config;

public class DbAutoCommitBehavior : ITransactionalBehavior
{
    public CommitBehavior CommitBehavior => CommitBehavior.DbAuto;
}