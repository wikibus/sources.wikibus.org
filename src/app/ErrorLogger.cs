using System;
using Nancy;
using Nancy.Bootstrapper;

namespace Brochures.Wikibus.Org
{
    public class ErrorLogger : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.OnError.AddItemToEndOfPipeline(LogError);
        }

        private static object LogError(NancyContext ctx, Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);

            return ctx.Response;
        }
    }
}
