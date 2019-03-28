using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace RestMockCore {
    public class HttpServer : IHttpServer {
        private IWebHost _host;
        private readonly int _port;

        public IRequestBuilder Config { get; set; }

        public HttpServer(int port = 5000) {
            _port = port;
            Config = new RequestBuilder();
        }

        public void Run() {
            _host = WebHost.CreateDefaultBuilder()
                .UseUrls("http://localhost:" + _port.ToString())
                .Configure(app => {
                    app.Run(async context => {
                        string response = "It Works!";
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                        if (Config.RouteTable != null && Config.RouteTable.Count > 0) {
                            RouteTableItem route = null;
                            foreach (var item in Config.RouteTable) {
                                if (item.IsMatch(context.Request)) {
                                    route = item;
                                }
                            }
                            if (route != null) {
                                if (route.Response.Handler != null) {
                                    route.Response.Handler(context);
                                    return;
                                } else {
                                    response = route.Response.Body;
                                    context.Response.StatusCode = route.Response.StatusCode;
                                    if (route.Response.Headers != null) {
                                        foreach (var item in route.Response.Headers) {
                                            context.Response.Headers.Add(item.Key, item.Value);
                                        }
                                    }
                                    buffer = System.Text.Encoding.UTF8.GetBytes(response);
                                    await context.Response.Body.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                                    return;
                                }
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
