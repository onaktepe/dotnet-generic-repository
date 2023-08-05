using System.Security.Claims;
using DotNetBB.Repository.Abstraction.Interface;
using DotNetBB.Repository.EF.Config;
using DotNetBB.Repository.EF.Repository;
using DotNetBB.Repository.EF.UoW;
using SampleApp.Api.Contexts;
using SampleApp.Infra.Data;

namespace SampleApp.Api.Extensions;

public class ContextedRepository<TEntity>: Repository<TEntity, SampleDbContext>
    where TEntity: DotNetBB.Repository.Abstraction.Entity.BaseEntity
{
    public ContextedRepository(IUoW<SampleDbContext> uow, IUserContext userContext, ITransactionalBehavior transactionalBehavior, ILogger<Repository<TEntity, SampleDbContext>> logger): 
        base(uow, userContext, transactionalBehavior, logger)
    {
    }
}

public class ContextedCrudRepository<TEntity>: CrudRepository<TEntity, SampleDbContext>
    where TEntity: DotNetBB.Repository.Abstraction.Entity.CrudEntity
{
    public ContextedCrudRepository(IUoW<SampleDbContext> uow, IUserContext userContext, ITransactionalBehavior transactionalBehavior, ILogger<CrudRepository<TEntity, SampleDbContext>> logger): 
        base(uow, userContext, transactionalBehavior, logger)
    {
    }
}

public class ContextedQueryExecuter: QueryExecuter<SampleDbContext>
{
    public ContextedQueryExecuter(IUoW<SampleDbContext> uow, ITransactionalBehavior transactionalBehavior): 
        base(uow, transactionalBehavior)
    {
    }
}

public static class ServicesConfiguration
{
    private static readonly ILoggerFactory _EFLoggerFactory = LoggerFactory.Create(builder => {
        builder.AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name
                && level == LogLevel.Information)
            .AddConsole();
    });

    public static void ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IUoW<>), typeof(UoW<>));
        services.AddScoped(typeof(IRepository<>), typeof(ContextedRepository<>));
        services.AddScoped(typeof(ICrudRepository<>), typeof(ContextedCrudRepository<>));
        services.AddScoped(typeof(IQueryExecuter), typeof(ContextedQueryExecuter));
    }

    public static void ConfigureDbContext(this IServiceCollection services, string connectionString, bool enableSensitiveDataLogging)
    {
        services.AddSingleton<IContextFactory<SampleDbContext>, IContextFactory<SampleDbContext>>((ctx) =>
        {
            return new ContextFactory<SampleDbContext>(options => options.UseLoggerFactory(_EFLoggerFactory)
                .EnableSensitiveDataLogging()
                //.UseSqlServer(connectionString));
                .UseNpgsql(connectionString));
        });

        services.AddSingleton<IContextFactory<DbContext>, IContextFactory<SampleDbContext>>((ctx) => {
            var contextFactory = ctx.GetRequiredService<IContextFactory<SampleDbContext>>();
            return contextFactory;
        });

        services.AddSingleton<ITransactionalBehavior, DbScopedCommitBehavior>();
    }

    public static void ConfigureApplicationContext(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>((ctx) =>
        {
            var httpContextAccessor = ctx.GetRequiredService<IHttpContextAccessor>();
            int userId = 0;
            string? clientId = "";

            if (httpContextAccessor.HttpContext?.User?.Identity is not null)
            {
                string? userIdStr = ((ClaimsIdentity)httpContextAccessor.HttpContext?.User?.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(userIdStr, out userId);
            }
            if(httpContextAccessor.HttpContext?.Request != null)
            {
                clientId = httpContextAccessor.HttpContext.Request.Headers["ClientId"];
            }

            return new UserContext(userId.ToString(), clientId);
        });

    }

    public static void ConfigureDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
    }

}