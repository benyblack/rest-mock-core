using RestMockCore.Interfaces;
using RestMockCore.Models;

namespace RestMockCore
{
    public class RequestBuilder : IRequestBuilder
    {
        public IRequestHandler Handler { get; set; }

        public List<RouteTableItem> RouteTable { get; }

        public RequestBuilder()
        {
            Handler = new RequestHandler(null);
            RouteTable = new List<RouteTableItem>();
        }

        public IRequestHandler Delete(string url)
        {
            return Request("DELETE", url, null);
        }

        public IRequestHandler Get(string url)
        {
            return Request("GET", url, null);
        }

        public IRequestHandler Post(string url)
        {
            return Request("POST", url, null);
        }

        public IRequestHandler Put(string url)
        {
            return Request("PUT", url, null);
        }

        public IRequestHandler Request(string method, string url)
        {
            return Request(method, url, null);
        }

        public IRequestHandler Request(string method, string url, Dictionary<string, string> headers)
        {
            if (!url.StartsWith("/"))
            {
                url = $"/{url}";
            }
            RouteTable.Add(new RouteTableItem(method, url, headers));
            Handler = new RequestHandler(RouteTable.Last());
            return Handler;
        }

        public void VerifyAll()
        {
            RouteTable.ForEach(x => x.Verify());
        }
    }
}
