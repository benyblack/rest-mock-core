using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace RestMockCore
{
    public class HttpResponse : IHttpResponse
    {
        public string Body { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string,string> Headers { get; set; }

        public Action<HttpContext> Handler { get; set; }

    }
}
