using RestMockCore.Interfaces;

namespace RestMockCore.Models
{
    public class HttpRequest : IHttpRequest
    {
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public Dictionary<string, string>? Headers { get; set; }
    }
}
