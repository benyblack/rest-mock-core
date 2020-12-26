using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
                        var response = "It Works!";
                        var buffer = System.Text.Encoding.UTF8.GetBytes(response);
                        if (Config.RouteTable.HasAny())
                        {
                            var route = Config.RouteTable.LastOrDefault(x => x.IsMatch(context.Request));
                            if (route != null)
                            {
                                if (route.Response.Handler != null)
                                {
                                    route.Response.Handler(context);
                                }
                                else
                                {
                                    response = route.Response.Body;
                                    context.Response.StatusCode = route.Response.StatusCode;
                                    context.Response.Headers.AddRange(route.Response.Headers);
                                    buffer = System.Text.Encoding.UTF8.GetBytes(response);
                                    await context.Response.Body.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                                }
                                return;
                            }
                        }
                        context.Response.ContentLength = response.Length;
                        context.Response.ContentType = "text/plain";
                        await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
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
