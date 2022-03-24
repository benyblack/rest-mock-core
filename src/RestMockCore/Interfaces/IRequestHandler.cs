using RestMockCore.Models;
using HttpResponse = RestMockCore.Models.HttpResponse;

namespace RestMockCore.Interfaces;
public interface IRequestHandler
{
    RouteTableItem RouteTable { get; }
    HttpResponse Response { get; }
    RouteTableItem Send(string body);
    RouteTableItem Send(string body, int statusCode);
    RouteTableItem Send(string body, HttpStatusCode statusCode);
    RouteTableItem Send(string body, int statusCode, Dictionary<string, string> headers);
    RouteTableItem Send(string body, HttpStatusCode statusCode, Dictionary<string, string> headers);
    RouteTableItem Send(Action<HttpContext> context);
}
