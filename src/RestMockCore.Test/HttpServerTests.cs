using Xunit;
using RestMockCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace RestMockCore.Test
{
    public class HttpServerTests
    {
        private HttpServer _mockServer;

        [Fact]
        public async Task Test_general_functionality()
        {
            _mockServer = new HttpServer();
            _mockServer.Run();
            await send_request(5000);
        }

        [Fact]
        public async Task Test_constructor()
        {
            _mockServer = new HttpServer(5001);
            _mockServer.Run();
            await send_request(5001);
            var httpClient = new HttpClient();
            var responseGet = await httpClient.GetAsync(string.Format("http://localhost:{0}/", 5001));
            Assert.Equal("text/plain", responseGet.Content.Headers.GetValues("Content-Type").First());
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
            {
                httpClient.GetAsync("http://localhost:5000/").RunSynchronously();
            });
            Assert.NotNull(ex);

        }

        [Fact]
        public async Task Supports_disposable()
        {
            using (_mockServer = new HttpServer(5001))
            {
                _mockServer.Run();
            }
        }

        private async Task send_request(int port)
        {
            var httpClient = new HttpClient();

            var responseGetDefault = await httpClient.GetAsync(string.Format("http://localhost:{0}/", port));
            Assert.Equal("It Works!", await responseGetDefault.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)responseGetDefault.StatusCode);
            //=====================================
            var headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"},
                {"testHeader", "TestHeaderValue"}
            };

            _mockServer.Config.Get("/test/123").Send("It Really Works!");
            _mockServer.Config.Post("/havij/123").Send("It Not Works!", 503);
            _mockServer.Config.Put("/testput/456").Send("{'status':'isWorking'}", 200, headers);
            _mockServer.Config.Delete("/testdel/456").Send("Deleted", 200);
            _mockServer.Config.Get("/testAction/123").Send(context =>
            {
                context.Response.StatusCode = 200;
                string response = "Action Test";
                var buffer = System.Text.Encoding.UTF8.GetBytes(response);
                context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
            });

            Assert.NotNull(_mockServer.Config);
            Assert.Equal(5, _mockServer.Config.RouteTable.Count);

            var responseGet = await httpClient.GetAsync(string.Format("http://localhost:{0}/test/123", port));
            Assert.Equal("It Really Works!", await responseGet.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)responseGet.StatusCode);
            //================================================================
            HttpContent postData = new StringContent("{'data':'Test'}");
            var responsePost = await httpClient.PostAsync(string.Format("http://localhost:{0}/havij/123", port), postData);
            Assert.Equal("It Not Works!", await responsePost.Content.ReadAsStringAsync());
            Assert.Equal(503, (int)responsePost.StatusCode);
            //================================================================
            HttpContent putData = new StringContent("{'data':'Test'}");
            HttpRequestMessage putMessage = new HttpRequestMessage(HttpMethod.Put,
                string.Format("http://localhost:{0}/testput/456", port)) {Content = putData};
            var responsePut = await httpClient.SendAsync(putMessage);
            Assert.Equal("{'status':'isWorking'}", await responsePut.Content.ReadAsStringAsync());
            Assert.Equal("application/json", responsePut.Content.Headers.GetValues("Content-Type").First());
            Assert.Equal("TestHeaderValue", responsePut.Headers.GetValues("testHeader").First());
            Assert.Equal(200, (int)responsePut.StatusCode);
            //================================================================
            HttpRequestMessage deleteMessage = new HttpRequestMessage(HttpMethod.Delete, string.Format("http://localhost:{0}/testdel/456", port));
            var responseDelete = await httpClient.SendAsync(deleteMessage);
            Assert.Equal("Deleted", await responseDelete.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)responseDelete.StatusCode);
            //==========================================================
            var responseGetAction = await httpClient.GetAsync(string.Format("http://localhost:{0}/testAction/123", port));
            Assert.Equal("Action Test", await responseGetAction.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)responseGetAction.StatusCode);

        }


    }
}
