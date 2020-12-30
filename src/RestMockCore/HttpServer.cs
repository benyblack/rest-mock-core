using System.Linq;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RestMockCore.Interfaces;
using RestMockCore.Models;

namespace RestMockCore
{
    public class HttpServer : IHttpServer
    {
        private IWebHost _host;
        private readonly int _port;

        public IRequestBuilder Config { get; set; }

        public HttpServer(int port = 5000)
        {
            _port = port;
            Config = new RequestBuilder();
        }

        public void Run()
        {
            _host = WebHost.CreateDefaultBuilder()
                .UseUrls($"http://localhost:{_port}")
                .Configure(app =>
                {
                    app.Run(async context =>
                    {
                        if (context.Request.Path == @"/")
                        {
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync("It Works!", Encoding.UTF8);
                            return;
                        }

                        var route = Config.RouteTable?.LastOrDefault(x => x.IsMatch(context.Request));
                        if (route == null)
                        {
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync("Page not found!", Encoding.UTF8);
                            return;
                        }

                        if (route.Response.Handler != null)
                        {
                            route.Response.Handler(context);
                            return;
                        }

                        var response = route.Response.Body;
                        context.Response.StatusCode = route.Response.StatusCode;
                        context.Response.Headers.AddRange(route.Response.Headers);
                        await context.Response.WriteAsync(response, Encoding.UTF8).ConfigureAwait(false);
                    });
                }).Build();
            _host.Start();
        }

        public void Dispose()
        {
            _host?.Dispose();
        }
    }
}
