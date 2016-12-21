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

        IRequestHandler _handler = null;
        public IRequestHandler Handler
        {
            get
            {
                if (_handler == null)
                {
                    _handler = new RequestHandler(null);
                }
                return _handler;
            }
        }

        List<RouteTableItem> _routeTable = null;
        public List<RouteTableItem> RouteTable
        {
            get
            {
                if (_routeTable == null)
                {
                    _routeTable = new List<RouteTableItem>();
                }
                return _routeTable;
            }
        }

        #region Methods
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
        #endregion

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
