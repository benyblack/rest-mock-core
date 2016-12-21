using Xunit;
using RestMockCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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

            HttpClient httpClient = new HttpClient();

            var response1 = await httpClient.GetAsync("http://localhost:5000/");
            Assert.Equal("It Works!", await response1.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)response1.StatusCode);
            //=====================================
            mockServer.Config.Get("/test/123").Send("It Really Works!");
            mockServer.Config.Post("/havij/123").Send("It Not Works!", 503);

            Assert.NotNull(mockServer.Config);
            Assert.Equal(2, mockServer.Config.RouteTable.Count);

            var response2 = await httpClient.GetAsync("http://localhost:5000/test/123");
            Assert.Equal("It Really Works!", await response2.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)response2.StatusCode);
            //================================================================
            HttpContent postData = new StringContent("{'data':'Test'}");
            var response3 = await httpClient.PostAsync("http://localhost:5000/havij/123", postData);
            Assert.Equal("It Not Works!", await response3.Content.ReadAsStringAsync());
            Assert.Equal(503, (int)response3.StatusCode);


        }

    }
}
