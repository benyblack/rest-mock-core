using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestMockCore.Test
{
    public class RequestHandlerTest : IDisposable
    {
        RequestHandler requestHandler;
        RouteTableItem route;
        public RequestHandlerTest()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            route = new RouteTableItem("GET", "/test/123/", headers);
            requestHandler = new RequestHandler(route);
        }

        [Fact]
        public void SendTest()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            requestHandler.Send("test body", 503, headers);
            Assert.NotNull(route.Response);
            Assert.Equal("test body", route.Response.Body);
            Assert.Equal(503, route.Response.StatusCode);
            Assert.Equal(1, route.Response.Headers.Count);
            Assert.Equal("application/json", route.Response.Headers["Content-Type"]);
        }

        [Fact]
        public void SendTest_Action()
        {
            requestHandler.Send(context=> {
                context.ToString();
            });
            Assert.NotNull(route.Response);
            Assert.NotNull(route.Response.Handler);
        }

        [Fact]
        public void Response_Getter_Test()
        {
            var response = requestHandler.Response;
            Assert.NotNull(response);
        }

        public void Dispose()
        {

        }
    }
}
