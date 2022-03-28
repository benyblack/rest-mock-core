using Moq;
using RestMockCore.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace RestMockCore.Test.Models
{
    public class RouteTableItemTest
    {
        private readonly Dictionary<string, string> _headers;
        private const string URL_WITH_QUERY = "/test/123/?test=123";
        private const string URL_WITHOUT_QUERY = "/test/123/";
        private const string GET = "GET";
        private const string POST = "POST";
        private readonly Mock<Microsoft.AspNetCore.Http.HttpRequest> _httpRequestMock;

        public RouteTableItemTest()
        {
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
            _httpRequestMock.Setup(request => request.Path).Returns("/test/123/");
            _httpRequestMock.Setup(request => request.QueryString).Returns(new QueryString("?test=123"));
        }

        [Fact]
        public void Verify_ShouldThrowException_WhenRouteIsNotCalled()
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = true;
            route.IsCalled = false;
            //Act
            var exception = Assert.Throws<Exception>(() => route.Verify());
            //Assert
            Assert.Equal("Route is not verifiable", exception.Message);
        }

        [Fact]
        public void Verify_ShouldNotThrowException_WhenRouteIsNotVerifiable()
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = false;
            route.IsCalled = false;
            //Act
            route.Verify();

            //Assert

        }

        [Fact]
        public void Verify_ShouldNotThrowException_WhenRouteIsVerifiableAndIsCalled()
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = true;
            route.IsCalled = true;
            //Act
            route.Verify();

            //Assert

        }

        [Fact]
        public void VerifiableExtMethod_ShouldChangeIsVerifiableToTrue()
        {
            //Arrange
            var routeTableItem = new RouteTableItem();
            var RequestHandler = new RequestHandler(routeTableItem);

            //Act
            _ = RequestHandler.Send("something").Verifiable();

            //Assert
            Assert.True(routeTableItem.IsVerifiable);
        }

        [Fact]
        public void Constructor_Should_Work_Correctly()
        {
            //Act
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);

            //Assert
            Assert.NotNull(route.Request);
            Assert.Equal(GET, route.Request.Method);
            Assert.Equal(URL_WITH_QUERY, route.Request.Url);
            Assert.Equal(_headers, route.Request.Headers);
        }

        [Fact]
        public void IsMatch_Should_Return_True_On_Valid_Data()
        {
            //Act
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);

            //Assert
            Assert.True(route.IsMatch(_httpRequestMock.Object));
        }

        [Fact]
        public void IsMatch_Should_Return_False_On_Different_Method()
        {
            //Act
            var route = new RouteTableItem(POST, URL_WITH_QUERY, _headers);

            //Assert
            Assert.False(route.IsMatch(_httpRequestMock.Object));
        }

        [Fact]
        public void IsMatch_Should_Return_False_On_Different_Url()
        {
            //Act
            var route = new RouteTableItem(GET, URL_WITHOUT_QUERY, _headers);

            //Assert
            Assert.False(route.IsMatch(_httpRequestMock.Object));
        }

        [Fact]
        public void IsMatch_Should_Return_True_On_ValidData_With_Null_Or_Empty_Header()
        {
            //Act
            var route = new RouteTableItem(GET, URL_WITH_QUERY, null);
            var route1 = new RouteTableItem(GET, URL_WITH_QUERY, new Dictionary<string, string>());

            //Assert
            Assert.True(route.IsMatch(_httpRequestMock.Object));
            Assert.True(route1.IsMatch(_httpRequestMock.Object));
        }

        [Theory]
        [ClassData(typeof(HeaderData))]
        public void IsMatch_Should_Work_Correctly_With_Different_Header(bool expected, Dictionary<string, string> headers)
        {
            //Act
            var route = new RouteTableItem(GET, URL_WITH_QUERY, headers);

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
