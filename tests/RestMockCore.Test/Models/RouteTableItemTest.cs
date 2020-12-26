using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Collections.Generic;
using RestMockCore.Models;
using Xunit;

namespace RestMockCore.Test.Models
{
    public class RouteTableItemTest
    {
        private readonly Dictionary<string, string> _headers;
        private readonly Mock<Microsoft.AspNetCore.Http.HttpRequest> _httpRequestMock;

        public RouteTableItemTest()
        {
            //Arrange
            _headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"},
                {"testHeader", "TestHeaderValue"}
            };

            var headerDictionary = new HeaderDictionary
            {
                {"Content-Type", new StringValues("application/json")},
                {"testHeader", new StringValues("TestHeaderValue")}
            };

            _httpRequestMock = new Mock<Microsoft.AspNetCore.Http.HttpRequest>();
            _httpRequestMock.Setup(request => request.Headers).Returns(headerDictionary);
            _httpRequestMock.Setup(request => request.Method).Returns("GET");
            _httpRequestMock.Setup(request => request.Path).Returns("/test");
            _httpRequestMock.Setup(request => request.QueryString).Returns(new QueryString("?q=123"));
        }

        [Fact]
        public void Constructor_Should_Work_Correctly()
        {
            //Act
            var route = new RouteTableItem("Get", "test/test", _headers);

            //Assert
            Assert.NotNull(route.Request);
            Assert.Equal("Get", route.Request.Method);
            Assert.Equal("test/test", route.Request.Url);
            Assert.Equal(_headers, route.Request.Headers);
        }

        [Fact]
        public void IsMatch_Should_Return_True_On_Valid_Data()
        {
            //Act
            var route = new RouteTableItem("GET", "/test?q=123", _headers);
           
            //Assert
            Assert.True(route.IsMatch(_httpRequestMock.Object));
        }

        [Fact]
        public void IsMatch_Should_Return_False_On_Different_Method()
        {
            //Act
            var route = new RouteTableItem("POST", "/test?q=123", _headers);

            //Assert
            Assert.False(route.IsMatch(_httpRequestMock.Object));
        }

        [Fact]
        public void IsMatch_Should_Return_False_On_Different_Url()
        {
            //Act
            var route = new RouteTableItem("GET", "/test2?q=123", _headers);

            //Assert
            Assert.False(route.IsMatch(_httpRequestMock.Object));
        }

        [Fact]
        public void IsMatch_Should_Return_True_On_ValidData_With_Null_Or_Empty_Header()
        {
            //Act
            var route = new RouteTableItem("GET", "/test?q=123", null);
            var route1 = new RouteTableItem("GET", "/test?q=123", new Dictionary<string, string>());

            //Assert
            Assert.True(route.IsMatch(_httpRequestMock.Object));
            Assert.True(route1.IsMatch(_httpRequestMock.Object));
        }

        [Theory]
        [ClassData(typeof(HeaderData))]
        public void IsMatch_Should_Work_Correctly_With_Different_Header(bool expected, Dictionary<string, string> headers)
        {
            //Act
            var route = new RouteTableItem("GET", "/test?q=123", headers);

            //Assert
            Assert.Equal(expected, route.IsMatch(_httpRequestMock.Object));
        }
    }

    public class HeaderData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                true, new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"},
                    {"testHeader", "TestHeaderValue"}
                }
            };
            yield return new object[]
            {
                false, new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"},
                    {"testHeader", "TestHeaderValue2"}
                }
            };
            yield return new object[]
            {
                false, new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"},
                    {"testKey", "TestHeaderValue"}
                }
            };
            yield return new object[]
            {
                false, new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"},
                    {"testHeader", "TestHeaderValue"},
                    {"testKey", "TestHeaderValue"}
                }
            };
            yield return new object[]
            {
                true, new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }
            };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
