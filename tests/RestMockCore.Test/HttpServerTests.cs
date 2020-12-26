using Xunit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace RestMockCore.Test
{
    public class HttpServerTests
    {
        private HttpServer _mockServer;
        private readonly Dictionary<string, string> _headers;
        private readonly HttpClient _httpClient;
        private const int Port = 5000;
        private readonly string _address = $"http://localhost:{Port}";

        public HttpServerTests()
        {
            //Arrange
            _headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"},
                {"testHeader", "TestHeaderValue"}
            };

            _httpClient = new HttpClient();

        }

        [Fact]
        public async void Server_With_No_RouteTable_Should_Return_Default_Response()
        {
            //Arrange
            _mockServer = new HttpServer(Port);
            _mockServer.Run();

            //Act
            var defaultResponse = await _httpClient.GetAsync(_address);
            _mockServer.Dispose();

            //Assert
            Assert.Equal("It Works!", await defaultResponse.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)defaultResponse.StatusCode);
            Assert.Equal("text/plain", defaultResponse.Content.Headers.GetValues("Content-Type").First());
            Assert.Throws<InvalidOperationException>(() => _httpClient.GetAsync(_address).RunSynchronously());
        }

        [Fact]
        public async void Request_With_No_Match_RouteTable_Should_Return_PageNotFound()
        {
            //Arrange
            _mockServer = new HttpServer(Port);
            _mockServer.Run();

            //Act
            _mockServer.Config.Get("/test/123").Send("It Really Works!");
            var responseGet = await _httpClient.GetAsync($"{_address}/test/456");
            _mockServer.Dispose();

            //Assert
            Assert.Equal("Page not found!", await responseGet.Content.ReadAsStringAsync());
            Assert.Equal(404, (int)responseGet.StatusCode);
            Assert.Equal("text/plain", responseGet.Content.Headers.GetValues("Content-Type").First());
            Assert.Throws<InvalidOperationException>(() => _httpClient.GetAsync(_address).RunSynchronously());
        }

        [Fact]
        public async void Request_Should_Return_PageNotFound_When_No_Route_Has_Been_Added()
        {
            //Arrange
            _mockServer = new HttpServer(Port);
            _mockServer.Run();

            //Act
            var responseGet = await _httpClient.GetAsync($"{_address}/test/456");
            _mockServer.Dispose();

            //Assert
            Assert.Equal("Page not found!", await responseGet.Content.ReadAsStringAsync());
            Assert.Equal(404, (int)responseGet.StatusCode);
            Assert.Equal("text/plain", responseGet.Content.Headers.GetValues("Content-Type").First());
            Assert.Throws<InvalidOperationException>(() => _httpClient.GetAsync(_address).RunSynchronously());
        }

        [Fact]
        public async void MockServer_Should_Supports_IDisposable()
        {
            //Arrange
            using (_mockServer = new HttpServer(Port))
            {
                //Act
                _mockServer.Run();
                var defaultResponse = await _httpClient.GetAsync(_address);
                _mockServer.Dispose();

                //Assert
                Assert.Equal("It Works!", await defaultResponse.Content.ReadAsStringAsync());
                Assert.Equal(200, (int)defaultResponse.StatusCode);
                Assert.Equal("text/plain", defaultResponse.Content.Headers.GetValues("Content-Type").First());
            }
        }

        [Fact]
        public void Add_RouteTableItem_Should_Work_Correctly()
        {
            //Arrange
            _mockServer = new HttpServer(Port);
            _mockServer.Run();

            //Act
            _mockServer.Config.Get("/test/123").Send("It Really Works!");
            _mockServer.Config.Get("/testAction/123").Send(context =>
            {
                context.Response.StatusCode = 200;
                const string response = "Action Test";
                var buffer = System.Text.Encoding.UTF8.GetBytes(response);
                context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
            });
            _mockServer.Config.Post("/testPost/123").Send("It Not Works!", 503);
            _mockServer.Config.Put("/testPut/123").Send("Internal Server Error!", 500);
            _mockServer.Dispose();

            //Assert
            Assert.NotNull(_mockServer.Config);
            Assert.Equal(4, _mockServer.Config.RouteTable.Count);

            Assert.Equal("/test/123", _mockServer.Config.RouteTable[0].Request.Url);
            Assert.Equal("/testAction/123", _mockServer.Config.RouteTable[1].Request.Url);
            Assert.Equal("/testPost/123", _mockServer.Config.RouteTable[2].Request.Url);
            Assert.Equal("/testPut/123", _mockServer.Config.RouteTable[3].Request.Url);
        }

        [Fact]
        public async void Get_Should_Work_Correctly()
        {
            //Arrange
            _mockServer = new HttpServer(Port);
            _mockServer.Run();

            //Act
            _mockServer.Config.Get("/test/123").Send("It Really Works!");
            var responseGet = await _httpClient.GetAsync($"{_address}/test/123");

            _mockServer.Config.Get("/testAction/123").Send(context =>
            {
                context.Response.StatusCode = 200;
                const string response = "Action Test";
                var buffer = System.Text.Encoding.UTF8.GetBytes(response);
                context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
            });
            var responseGetAction = await _httpClient.GetAsync($"{_address}/testAction/123");
            _mockServer.Dispose();

            //Assert
            Assert.Equal("It Really Works!", await responseGet.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)responseGet.StatusCode);

            Assert.Equal("Action Test", await responseGetAction.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)responseGetAction.StatusCode);
        }

        [Fact]
        public async void Post_Should_Work_Correctly()
        {
            //Arrange
            _mockServer = new HttpServer(Port);
            _mockServer.Run();

            //Act
            _mockServer.Config.Post("/havij/123").Send("It Not Works!", 503);
            HttpContent postData = new StringContent("{'data':'Test'}");
            var responsePost = await _httpClient.PostAsync($"{_address}/havij/123", postData);
            _mockServer.Dispose();

            //Assert
            Assert.Equal("It Not Works!", await responsePost.Content.ReadAsStringAsync());
            Assert.Equal(503, (int)responsePost.StatusCode);
        }

        [Fact]
        public async void Put_Should_Work_Correctly()
        {
            //Arrange
            _mockServer = new HttpServer(Port);
            _mockServer.Run();

            //Act
            _mockServer.Config.Put("/testPut/456").Send("{'status':'isWorking'}", 200, _headers);
            HttpContent putData = new StringContent("{'data':'Test'}");
            var putMessage = new HttpRequestMessage(HttpMethod.Put, $"{_address}/testPut/456") { Content = putData };
            var responsePut = await _httpClient.SendAsync(putMessage);
            _mockServer.Dispose();

            //Assert
            Assert.Equal("{'status':'isWorking'}", await responsePut.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)responsePut.StatusCode);
            Assert.Equal("application/json", responsePut.Content.Headers.GetValues("Content-Type").First());
            Assert.Equal("TestHeaderValue", responsePut.Headers.GetValues("testHeader").First());
        }

        [Fact]
        public async void Delete_Should_Work_Correctly()
        {
            //Arrange
            _mockServer = new HttpServer(Port);
            _mockServer.Run();

            //Act
            _mockServer.Config.Delete("/testDel/456").Send("Deleted", 200);
            var deleteMessage = new HttpRequestMessage(HttpMethod.Delete, $"{_address}/testDel/456");
            var responseDelete = await _httpClient.SendAsync(deleteMessage);
            _mockServer.Dispose();

            //Assert
            Assert.Equal("Deleted", await responseDelete.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)responseDelete.StatusCode);
        }
    }
}
