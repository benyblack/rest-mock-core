using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RestMockCore.Interfaces;
using System.Threading.Tasks;

namespace RestMockCore;
public class HttpServer : IHttpServer
{
    private IHost _host;
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
        _host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel()
                    .UseUrls($"http://{_hostname}:{_port}")
                    .Configure(app =>
                    {
                        app.Run(async context =>
                        {
                            var routeTable = Config.RouteTable;
                            RestMockCore.Models.RouteTableItem route = null;
                            if (routeTable != null)
                            {
                                for (int i = routeTable.Count - 1; i >= 0; i--)
                                {
                                    var candidate = routeTable[i];
                                    if (candidate.IsMatch(context.Request))
                                    {
                                        route = candidate;
                                        break;
                                    }
                                }
                            }

                            if (route == null)
                            {
                                await HandleRouteNotFound(context).ConfigureAwait(false);
                                return;
                            }

                            // Now we have something to handle the request
                            // and we can verify the route is going to be handled
                            route.IncrementCallCounter();

                            if (route.Response.Handler != null)
                            {
                                route.Response.Handler(context);
                                return;
                            }

                            var responseText = route.Response.Body;
                            context.Response.StatusCode = (int)route.Response.StatusCode;
                            context.Response.Headers.AddRange(route.Response.Headers);
                            await context.Response.WriteAsync(responseText, Encoding.UTF8).ConfigureAwait(false);
                        });
                    });
            })
            .Build();
        _host.Start();
    }

    private static async Task HandleRouteNotFound(HttpContext context)
    {
        if (context.Request.Path == @"/")
        {
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("It Works!", Encoding.UTF8).ConfigureAwait(false);
            return;
        }
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Page not found!", Encoding.UTF8).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _host?.Dispose();
    }
}
