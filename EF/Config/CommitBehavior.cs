namespace DotNetBB.Repository.EF.Config;

public enum CommitBehavior
{
    ContextScoped = 1,
    ContextAuto = 2,
    DbScoped = 3,
    DbAuto = 4,
    DbBatch = 5,
}