using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using RestMockCore.Interfaces;
using RestMockCore.Models;

namespace RestMockCore;
public class HttpServer : IHttpServer
{
    private IWebHost _host;
    private readonly string _hostname;
    private readonly int _port;

    public IRequestBuilder Config { get; set; }

    public HttpServer(int port = 5000, string hostname = "localhost")
    {
        _hostname = hostname;
        _port = port;
        Config = new RequestBuilder();
    }

    public void Run()
    {
        _host = new WebHostBuilder()
            .UseKestrel()
            .UseUrls($"http://{_hostname}:{_port}")
            .Configure(app =>
            {
                app.Run(async context =>
                {
                    var route = Config.RouteTable?.LastOrDefault(x => x.IsMatch(context.Request));

                    if (route == null)
                    {
                        HandleRouteNotFound(context);
                        return;
                    }

                    if (route.Response.Handler != null)
                    {
                        route.Response.Handler(context);
                        return;
                    }

                    var responseText = route.Response.Body;
                    context.Response.StatusCode = route.Response.StatusCode;
                    context.Response.Headers.AddRange(route.Response.Headers);
                    await context.Response.WriteAsync(responseText, Encoding.UTF8).ConfigureAwait(false);
                });
            }).Build();
        _host.Start();
    }

    private async void HandleRouteNotFound(HttpContext context)
    {
        if (context.Request.Path == @"/")
        {
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("It Works!", Encoding.UTF8);
            return;
        }
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Page not found!", Encoding.UTF8);
    }

    public void Dispose()
    {
        _host?.Dispose();
    }
}
