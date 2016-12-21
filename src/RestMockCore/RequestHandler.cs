﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestMockCore
{
    public class RequestHandler : IRequestHandler
    {

        RouteTableItem _route;
        public RequestHandler(RouteTableItem route)
        {
            _route = route;
        }

        HttpResponse _response = null;
        public HttpResponse Response
        {
            get
            {
                if (_response == null)
                {
                    _response = new HttpResponse();
                }
                return _response;
            }
        }

        public void Send(string body)
        {
            Send(body, 200, null);
        }

        public void Send(string body, int statusCode)
        {
            Send(body, statusCode, null);
        }

        public void Send(string body, int statusCode, Dictionary<string, string> headers)
        {
            _response = new HttpResponse() { Body = body, StatusCode = statusCode, Headers = headers };
            _route.Response = _response;
        }
    }
}
