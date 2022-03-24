namespace RestMockCore.Models
{
    public class RouteTableItem
    {
        public HttpResponse Response { get; set; }
        public HttpRequest Request { get; set; }
        public bool IsVerifiable { get; set; } = false;
        public bool IsCalled { get; set; } = false;
        public void Verify()
        {
            if (IsVerifiable && !IsCalled)
            {
                throw new Exception("Route is not verifiable");
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
