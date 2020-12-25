using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using RestMockCore.Models;
using HttpResponse = RestMockCore.Models.HttpResponse;

namespace RestMockCore.Interfaces
{
    public interface IRequestHandler
    {
        RouteTableItem RouteTable { get; }
        HttpResponse Response { get; }
        void Send(string body);
        void Send(string body, int statusCode);
        void Send(string body, int statusCode, Dictionary<string, string> headers);
        void Send(Action<HttpContext> context);
    }
}
