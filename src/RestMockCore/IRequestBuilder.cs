using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestMockCore
{
    public interface IRequestBuilder
    {
        List<RouteTableItem> RouteTable { get;  }
      
        IRequestHandler Get(string url);
        IRequestHandler Post(string url);
        IRequestHandler Put(string url);
        IRequestHandler Delete(string url);

        IRequestHandler Request(string method, string url);
        IRequestHandler Request(string method, string url, Dictionary<string, string> headers);

    }
}
