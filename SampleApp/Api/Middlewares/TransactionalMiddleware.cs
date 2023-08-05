using DotNetBB.Repository.Abstraction.Interface;
using SampleApp.Infra.Data;

namespace SampleApp.Api.Middlewares;

public class TransactionalMiddleware
{
    private readonly RequestDelegate _next;

    public TransactionalMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUoW<SampleDbContext> uoW, ILogger<TransactionalMiddleware> logger)
    {
        Stream originalBody = context.Response.Body;
        var isCommitted = false;

        try
        {
            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                await _next(context);

                memoryStream.Position = 0;
                string responseBody = new StreamReader(memoryStream).ReadToEnd();

                isCommitted = FinishTransaction(uoW, logger, false, context.Request?.Path.ToString(), context.Response?.StatusCode);

                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBody);
            }

        }
        catch (Exception ex)
        {
            isCommitted = FinishTransaction(uoW, logger, true, context.Request?.Path.ToString(), context.Response?.StatusCode);
            throw;
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private bool FinishTransaction(IUoW<SampleDbContext> uoW,
        ILogger<TransactionalMiddleware> logger,
        bool isException,
        string? requestUrl,
        int? httpStatusCode)
    {
        bool isHttpStatusCodeError = httpStatusCode == StatusCodes.Status500InternalServerError;

        
        if (isException || isHttpStatusCodeError) //Internal server error
        {
            logger.LogDebug($"UOW rollbacking with status code:{httpStatusCode}, isException:{isException}, for url:{requestUrl}");
            uoW.Rollback();
            logger.LogDebug($"UOW rollbacked with for url:{requestUrl}");

            return true;
        }
        else
        {
            logger.LogDebug($"UOW committing for url:{requestUrl}");
            uoW.Commit();
            logger.LogDebug($"UOW committed for url:{requestUrl}");

            return false;
        }
    }

}
