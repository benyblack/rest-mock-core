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
            Method = "GET";
            Url = "";
            Headers = new Dictionary<string, string>();
        }

        public RouteTableItem(string method, string url, Dictionary<string, string> headers)
        {
            Method = method;
            Url = url;
            Headers = headers;
        }

        public HttpResponse Response {get;set;}

        public string Method { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> Headers { get; set; }

    }

}
