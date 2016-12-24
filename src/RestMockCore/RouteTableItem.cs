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

        public bool IsMatch(Microsoft.AspNetCore.Http.HttpRequest httpRequest) {
            if(Request.Method!= httpRequest.Method)
            {
                return false;
            }
            if (Request.Url == string.Format("{0}{1}", httpRequest.Path, httpRequest.QueryString))
            {
                if (Request.Headers != null && Request.Headers.Count > 0)
                {

                    foreach (var header in Request.Headers)
                    {
                        if (!httpRequest.Headers.Keys.Contains(header.Key))
                        {
                            return false;
                        }
                        else if (httpRequest.Headers[header.Key] != header.Value)
                        {
                            return false;
                        }
                    }

                }
                return true;
            }
            return false;
        }


    }

}
