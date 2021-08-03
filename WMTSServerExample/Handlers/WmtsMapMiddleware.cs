using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Handlers
{

    public class WmtsMapMiddleware : AbstractMapMiddleware
    {
        public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var request = context.Request;
                    var url = this.GetFixedUrl(request);
                    var description = this.GetDescription(url);
                    await WMTSServer.ParseQueryStringAsync(description, context);
                });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                throw;
            }

        }
    }
}