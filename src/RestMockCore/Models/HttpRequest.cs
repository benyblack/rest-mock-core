using System.Collections.Generic;
using RestMockCore.Interfaces;

namespace RestMockCore.Models
{
    public class HttpRequest : IHttpRequest
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
