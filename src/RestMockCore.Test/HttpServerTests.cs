using Xunit;
using RestMockCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace RestMockCore.Test
{
    public class HttpServerTests : IDisposable
    {
        HttpServer mockServer;
        public HttpServerTests()
        {
            mockServer = new HttpServer();
        }

        public void Dispose()
        {
        }

        [Fact]
        public async Task Test_general_functionality()
        {
            mockServer.Run();
            await send_request(5000);
        }

        private async Task send_request(int port)
        {
            HttpClient httpClient = new HttpClient();

            var responseGetDefault = await httpClient.GetAsync(string.Format("http://localhost:{0}/", port));
            Assert.Equal("It Works!", await responseGetDefault.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)responseGetDefault.StatusCode);
            //=====================================
            mockServer.Config.Get("/test/123").Send("It Really Works!");
            mockServer.Config.Post("/havij/123").Send("It Not Works!", 503);

            Assert.NotNull(mockServer.Config);
            Assert.Equal(2, mockServer.Config.RouteTable.Count);

            var responseGet = await httpClient.GetAsync(string.Format("http://localhost:{0}/test/123", port));
            Assert.Equal("It Really Works!", await responseGet.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)responseGet.StatusCode);
            //================================================================
            HttpContent postData = new StringContent("{'data':'Test'}");
            var responsePost = await httpClient.PostAsync(string.Format("http://localhost:{0}/havij/123", port), postData);
            Assert.Equal("It Not Works!", await responsePost.Content.ReadAsStringAsync());
            Assert.Equal(503, (int)responsePost.StatusCode);
        }

        [Fact]
        public async Task Test_constructor()
        {
            mockServer = new HttpServer(5001);
            mockServer.Run();
            await send_request(5001);
            HttpClient httpClient = new HttpClient();
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => { httpClient.GetAsync("http://localhost:5000/").RunSynchronously(); });
            Assert.NotNull(ex);

        }

    }
}
