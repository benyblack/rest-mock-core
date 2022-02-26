﻿using RestMockCore.Interfaces;

namespace RestMockCore.Models;
public class HttpResponse : IHttpResponse
{
    public string Body { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Action<HttpContext> Handler { get; set; }

}
