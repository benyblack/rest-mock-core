using RestMockCore.Interfaces;

namespace RestMockCore.Models;
public class RequestHandler : IRequestHandler
{
    public RouteTableItem RouteTable { get; }
    public HttpResponse Response { get; private set; }

    public RequestHandler(RouteTableItem route)
    {
        RouteTable = route;
        Response = new HttpResponse();
    }

    public void Send(string body)
    {
        Send(body, HttpStatusCode.OK, null);
    }

    public void Send(string body, int statusCode)
    {
        Send(body, (HttpStatusCode)statusCode, null);
    }

    public void Send(string body, HttpStatusCode statusCode)
    {
        Send(body, statusCode, null);
    }

    public void Send(Action<HttpContext> context)
    {
        Send("", HttpStatusCode.OK, null, context);
    }

    public void Send(string body, int statusCode, Dictionary<string, string> headers)
    {
        Send(body, statusCode, headers, null);
    }

    public void Send(string body, HttpStatusCode statusCode, Dictionary<string, string> headers)
    {
        Send(body, statusCode, headers, null);
    }

    public void Send(string body, int statusCode, Dictionary<string, string> headers, Action<HttpContext> context)
    {
        Send(body, (HttpStatusCode)statusCode, headers, context);
    }

    public void Send(string body, HttpStatusCode statusCode, Dictionary<string, string> headers, Action<HttpContext> context)
    {
        Response = new HttpResponse() { Body = body, StatusCode = statusCode, Headers = headers, Handler = context };
        RouteTable.Response = Response;
    }
}
