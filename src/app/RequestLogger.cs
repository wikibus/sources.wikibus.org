using System;
using Anotar.Serilog;
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
            LogTo.Information($"{ctx.Response.StatusCode} {ctx.Response.ReasonPhrase}");
        }

        private static Response LogRequestStart(NancyContext ctx)
        {
            LogTo.Information($"{ctx.Request.Method} {ctx.Request.Url}");
            return null;
        }

        private static object LogError(NancyContext ctx, Exception ex)
        {
            LogTo.Error(ex, "Request failed");

            return ctx.Response;
        }
    }
}
