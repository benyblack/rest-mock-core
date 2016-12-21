using System.Collections.Generic;

namespace RestMockCore
{
    public interface IHttpResponse
    {
        string Body { get; set; }
        Dictionary<string, string> Headers { get; set; }
        int StatusCode { get; set; }
    }
}