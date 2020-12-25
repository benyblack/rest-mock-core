using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using RestMockCore.Interfaces;

namespace RestMockCore.Models
{
    public class RequestHandler : IRequestHandler
    {
        private readonly RouteTableItem _route;
        public HttpResponse Response { get; private set; }

        public RequestHandler(RouteTableItem route)
        {
            _route = route;
            Response = new HttpResponse();
        }

        public void Send(string body)
        {
            Send(body, 200, null);
        }

        public void Send(string body, int statusCode)
        {
            Send(body, statusCode, null);
        }

        public void Send(Action<HttpContext> context)
        {
            Send("", 200, null, context);
        }

        public void Send(string body, int statusCode, Dictionary<string, string> headers)
        {
            Send(body, statusCode, headers, null);
        }

        public void Send(string body, int statusCode, Dictionary<string, string> headers, Action<HttpContext> context)
        {
            Response = new HttpResponse() { Body = body, StatusCode = statusCode, Headers = headers, Handler = context };
            _route.Response = Response;
        }
    }
}
