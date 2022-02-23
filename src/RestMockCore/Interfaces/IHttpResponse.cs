namespace RestMockCore.Interfaces;
public interface IHttpResponse
{
    string Body { get; set; }
    Dictionary<string, string> Headers { get; set; }
    int StatusCode { get; set; }
    Action<HttpContext> Handler { get; set; }

}
