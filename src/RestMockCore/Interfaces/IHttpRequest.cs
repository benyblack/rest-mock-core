using System.Collections.Generic;

namespace RestMockCore.Interfaces
{
    public interface IHttpRequest
    {
        Dictionary<string, string> Headers { get; set; }
        string Method { get; set; }
        string Url { get; set; }
    }
}
