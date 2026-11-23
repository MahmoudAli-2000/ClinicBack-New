using System.Text.Json;
using TourSite.APIs.Errors;

namespace TourSite.APIs.MidleWare
{
    public class ExceptionMidleWare
    {
        public RequestDelegate Next { get; }
        public ILogger<ExceptionMidleWare> Logger { get; }
        public IHostEnvironment Env { get; }
        public ExceptionMidleWare(RequestDelegate next,ILogger<ExceptionMidleWare> logger,IHostEnvironment env)
        {
            Next = next;
            Logger = logger;
            Env = env;
        }

        public async Task InvokeAsync(HttpContext Context) 
        {
            try
            {
                await Next.Invoke(Context);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,ex.Message);

                if (Context.Response.HasStarted) throw;
                Context.Response.ContentType = "application/json";
                var status = ex is TourSite.Core.Exceptions.AppException appError ? appError.StatusCode : 500;
                Context.Response.StatusCode = status;
                // Do not leak stack traces or SQL details to callers, even in development.
                var response = new APIErrerResponse(status, status == 500 ? TourSite.Core.Localization.Messages.Get("ServerError") : ex.Message);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response,options);
                await Context.Response.WriteAsync(json);
            }
        }

       
    }
}
