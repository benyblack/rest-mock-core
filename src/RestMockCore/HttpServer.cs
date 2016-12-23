using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace RestMockCore
{
    public class HttpServer : IHttpServer
    {
        IWebHost _host;
        int _port = 5000;

        public IRequestBuilder Config { get; set; }

        public HttpServer(int port = 5000)
        {
            _port = port;
            Config = new RequestBuilder();
        }
        public void Run()
        {
            new Task(() =>
            {
                _host = new WebHostBuilder()
                   .UseKestrel(options =>
                   {
                       options.NoDelay = true;
                       options.UseConnectionLogging();
                   })
                   .UseUrls("http://localhost:" + _port.ToString())
                   .Configure(app =>
                   {
                       app.Run(async context =>
                       {
                           if (Config.RouteTable != null && Config.RouteTable.Count > 0)
                           {
                               var routesByMethod = Config.RouteTable.Where(x => x.Method == context.Request.Method).ToList();
                               foreach (var item in routesByMethod)
                               {
                                   if (item.Url == string.Format("{0}{1}", context.Request.Path, context.Request.QueryString))
                                   {
                                       string response = item.Response.Body;
                                       context.Response.StatusCode = item.Response.StatusCode;
                                       byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                                       await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                                       break;
                                   }
                               }
                           }  
                           else
                           {
                               var response = "It Works!";
                               context.Response.ContentLength = response.Length;
                               context.Response.ContentType = "text/plain";
                               byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                               await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                           }
                       });
                   }).Build();

                _host.Run();

            }).Start();
        }
    }
}
