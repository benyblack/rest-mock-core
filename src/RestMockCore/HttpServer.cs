﻿using System;
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
                       string response = "It Works!";
                       byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                       if (Config.RouteTable != null && Config.RouteTable.Count > 0)
                       {
                           RouteTableItem route = MatchedRoute(context);
                           if (route != null)
                           {
                               response = route.Response.Body;
                               context.Response.StatusCode = route.Response.StatusCode;
                               if (route.Response.Headers != null)
                               {
                                   foreach (var item in route.Response.Headers)
                                   {
                                       context.Response.Headers.Add(item.Key, item.Value);
                                   }
                               }
                               buffer = System.Text.Encoding.UTF8.GetBytes(response);
                               await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
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

        private RouteTableItem MatchedRoute(HttpContext context)
        {
            var routesByMethod = Config.RouteTable.Where(x => x.Request.Method == context.Request.Method).ToList();
            foreach (var item in routesByMethod)
            {
                if (item.Request.Url == string.Format("{0}{1}", context.Request.Path, context.Request.QueryString))
                {
                    if (item.Request.Headers != null && item.Request.Headers.Count > 0)
                    {

                        foreach (var header in item.Request.Headers)
                        {
                            if (!context.Request.Headers.Keys.Contains(header.Key))
                            {
                                return null;
                            }
                            else if (context.Request.Headers[header.Key] != header.Value)
                            {
                                return null;
                            }
                        }

                    }
                    return item;
                }
            }
            return null;
        }
    }
}
