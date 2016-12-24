using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestMockCore
{
    public class RouteTableItem
    {
        public RouteTableItem()
        {
            Request = new HttpRequest();
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

        public HttpResponse Response { get; set; }
        public HttpRequest Request { get; set; }



    }

}
