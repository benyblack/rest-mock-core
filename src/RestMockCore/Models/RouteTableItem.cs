namespace RestMockCore.Models
{
    public class RouteTableItem
    {
        public HttpResponse Response { get; set; }
        public HttpRequest Request { get; set; }
        public bool IsVerifiable { get; set; } = false;
        public bool IsCalled => CallCounter > 0;
        public int CallCounter { get; set; } = 0;
        private const string NOT_VERIFIED = "Route can not be verified";
        public void Verify()
        {
            if (IsVerifiable && !IsCalled)
            {
                throw new Exception(NOT_VERIFIED);
            }
        }

        public void Verify(int times)
        {
            if (IsVerifiable && CallCounter != times)
            {
                throw new Exception($"{NOT_VERIFIED}, called {CallCounter} times");
            }
        }

        public void Verify(Func<int, bool> check)
        {
            if (IsVerifiable && !check(CallCounter))
            {
                throw new Exception(NOT_VERIFIED);
            }
        }

        public RouteTableItem() : this("", "", null)
        {
        }

        public RouteTableItem(string method, string url, Dictionary<string, string> headers)
        {
            Request = new HttpRequest()
            {
                Method = method,
                Url = url,
                Headers = headers
            };
        }

        public bool IsMatch(Microsoft.AspNetCore.Http.HttpRequest httpRequest)
        {
            if (Request.Method != httpRequest.Method) return false;
            if (Request.Url != $"{httpRequest.Path}{httpRequest.QueryString}") return false;
            if (!Request.Headers.HasAny()) return true;
            foreach (var header in Request.Headers)
            {
                if (!httpRequest.Headers.Keys.Contains(header.Key)) return false;
                if (httpRequest.Headers[header.Key] != header.Value) return false;
            }
            return true;
        }
    }
}
