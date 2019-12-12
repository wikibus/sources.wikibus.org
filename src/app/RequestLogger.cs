using System;
using Nancy;
using Nancy.Bootstrapper;

namespace Brochures.Wikibus.Org
{
    public class RequestLogger : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(LogRequestStart);
            pipelines.AfterRequest.AddItemToEndOfPipeline(LogResponse);
            pipelines.OnError.AddItemToEndOfPipeline(LogError);
        }

        private static void LogResponse(NancyContext ctx)
        {
            Console.WriteLine($"{ctx.Response.StatusCode} {ctx.Response.ReasonPhrase}");
        }

        private static Response LogRequestStart(NancyContext ctx)
        {
            Console.WriteLine($"{ctx.Request.Method} {ctx.Request.Url}");
            return null;
        }

        private static object LogError(NancyContext ctx, Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);

            return ctx.Response;
        }
    }
}
