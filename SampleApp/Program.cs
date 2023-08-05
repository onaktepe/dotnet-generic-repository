using DotNetBB.Repository.Abstraction.Interface;
using SampleApp.Api.Extensions;
using SampleApp.Infra.Data;
using SampleApp.Api.Middlewares;

namespace SampleApp;

public class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

        builder.Logging.ClearProviders()
            .AddConsole()
            .AddDebug();

        builder.Configuration.AddEnvironmentVariables()
            .AddCommandLine(args);

        // Add services to the container.
        var services = builder.Services;

        bool.TryParse(builder.Configuration["LogSensitiveData"], out bool logSensitiveData);
        services.ConfigureCoreServices();
        services.ConfigureDbContext(builder.Configuration["Database:ConnectionString"]?? throw new ArgumentNullException("Database:ConnectionString"),logSensitiveData);
        services.ConfigureApplicationContext();
        services.ConfigureDomainServices();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseDefaultFiles(); //rewrite url to index.html
        app.UseStaticFiles();  //default wwwroot as static files folder
                                // global cors policy
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<TransactionalMiddleware>();

        app.MapControllers();

        RunStartupJobs(app);

        app.Run();
    }

    private static void RunStartupJobs(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var hostEnvironment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
            if (!hostEnvironment.IsDevelopment()) return;

            var contextFactory = scope.ServiceProvider.GetRequiredService<IContextFactory<SampleDbContext>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var dbContext = contextFactory.Create();

            if (configuration.GetValue<bool>("Database:Create"))
            {
                var isCreated = dbContext.Database.EnsureCreated();
                
               /*  if (isCreated && dbContext.Database.IsSqlServer())
                {
                    dbContext.Database.ExecuteSqlRaw("ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON");
                    dbContext.Database.ExecuteSqlRaw("ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON");
                } */
            }

            if (configuration.GetValue<bool>("Database:Migrate"))
            {
                dbContext.Database.Migrate();
            }

        }
    }

}

