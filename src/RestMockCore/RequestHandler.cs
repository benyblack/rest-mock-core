using RestMockCore.Interfaces;
using RestMockCore.Models;
using HttpResponse = RestMockCore.Models.HttpResponse;

namespace RestMockCore;
public class RequestHandler : IRequestHandler
{
    public RouteTableItem RouteTable { get; }
    public HttpResponse Response { get; private set; }

    public RequestHandler(RouteTableItem route)
    {
        RouteTable = route;
        Response = new HttpResponse();
    }

    public RouteTableItem Send(string body)
    {
        return Send(body, HttpStatusCode.OK, null);
    }

    public RouteTableItem Send(string body, int statusCode)
    {
        return Send(body, (HttpStatusCode)statusCode);
    }

    public RouteTableItem Send(string body, HttpStatusCode statusCode)
    {
        return Send(body, statusCode, null);
    }

    public RouteTableItem Send(Action<HttpContext> context)
    {
        return Send("", HttpStatusCode.OK, null, context);
    }

    public RouteTableItem Send(string body, int statusCode, Dictionary<string, string> headers)
    {
        return Send(body, statusCode, headers, null);
    }

    public RouteTableItem Send(string body, HttpStatusCode statusCode, Dictionary<string, string> headers)
    {
        return Send(body, statusCode, headers, null);
    }

    public RouteTableItem Send(string body, int statusCode, Dictionary<string, string> headers, Action<HttpContext> context)
    {
        return Send(body, (HttpStatusCode)statusCode, headers, context);
    }

    public RouteTableItem Send(string body, HttpStatusCode statusCode, Dictionary<string, string> headers, Action<HttpContext> context)
    {
        Response = new HttpResponse() { Body = body, StatusCode = statusCode, Headers = headers, Handler = context };
        RouteTable.Response = Response;
        return RouteTable;
    }
}
