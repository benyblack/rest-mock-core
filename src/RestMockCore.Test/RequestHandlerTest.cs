using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace RestMockCore.Test
{
    public class RequestHandlerTest 
    {
        private readonly RequestHandler _requestHandler;
        private readonly RouteTableItem _route;
        public RequestHandlerTest()
        {
            var headers = new Dictionary<string, string> {{"Content-Type", "application/json"}};
            _route = new RouteTableItem("GET", "/test/123/", headers);
            _requestHandler = new RequestHandler(_route);
        }

        [Fact]
        public void SendTest()
        {
            var headers = new Dictionary<string, string> {{"Content-Type", "application/json"}};
            _requestHandler.Send("test body", 503, headers);
            Assert.NotNull(_route.Response);
            Assert.Equal("test body", _route.Response.Body);
            Assert.Equal(503, _route.Response.StatusCode);
            Assert.Single(_route.Response.Headers);
            Assert.Equal("application/json", _route.Response.Headers["Content-Type"]);
        }

        [Fact]
        public void SendTest_Action()
        {
            _requestHandler.Send(context => { });
            Assert.NotNull(_route.Response);
            Assert.NotNull(_route.Response.Handler);
        }

        [Fact]
        public void Response_Getter_Test()
        {
            var response = _requestHandler.Response;
            Assert.NotNull(response);
        }

    }
}
