using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestMockCore
{
    public class RequestBuilder : IRequestBuilder
    {
        public RequestBuilder()
        {
        }

        private IRequestHandler _handler = null;
        public IRequestHandler Handler => _handler ?? (_handler = new RequestHandler(null));
        private List<RouteTableItem> _routeTable = null;
        public List<RouteTableItem> RouteTable => _routeTable ?? (_routeTable = new List<RouteTableItem>());

        public IRequestHandler Delete(string url)
        {
            return Request("DELETE", url);
        }

        public IRequestHandler Get(string url)
        {
            return Request("GET", url);
        }

        public IRequestHandler Post(string url)
        {
            return Request("POST", url);
        }

        public IRequestHandler Put(string url)
        {
            return Request("PUT", url);
        }

        public IRequestHandler Request(string method, string url)
        {
            return Request(method, url, null);
        }

        public IRequestHandler Request(string method, string url, Dictionary<string, string> headers)
        {
            RouteTable.Add(new RouteTableItem(method, url, headers));
            _handler = new RequestHandler(RouteTable.Last());
            return _handler;
        }
    }
}
