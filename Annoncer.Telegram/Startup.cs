using System;
using System.IO;
using Annoncer.Telegram.Properties;
using Owin;

namespace Annoncer.Telegram
{
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/announce", builder =>
            {
                builder.Use(async (context, func) =>
                {
                    if (!string.Equals(context.Request.Method, "post", StringComparison.InvariantCultureIgnoreCase))
                    {
                        context.Response.StatusCode = 405;
                        await context.Response.WriteAsync("Method not allowed");
                        return;
                    }
                    if (context.Request.Headers["Authorization"] == Settings.Default.AuthValue)
                    {
                        var message = await new StreamReader(context.Request.Body).ReadToEndAsync();
                        await Program.Announce(message);
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("Done");
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Authentication failed");
                    }
                });
            });
        }
    }
}