using Moq;
using RestMockCore.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Xunit;
using AutoFixture;

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
        private readonly Fixture _fixture;

        public RouteTableItemTest()
        {
            _fixture = new Fixture();
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
            route.CallCounter = 0;

            //Act
            var exception = Assert.Throws<Exception>(() => route.Verify());

            //Assert
            Assert.Equal("Route can not be verified", exception.Message);
        }

        [Fact]
        public void Verify_ShouldNotThrowException_WhenRouteIsNotVerifiable()
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = false;
            route.CallCounter = 1;

            //Act & Assert
            route.Verify();
        }

        [Fact]
        public void Verify_ShouldNotThrowException_WhenRouteIsVerifiableAndIsCalled()
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = true;
            route.CallCounter = 1;

            //Act & Assert
            route.Verify();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(5)]
        public void Verify_ShouldNotThrowException_WhenRouteIsVerifiableAndIsCalledExactNumberOfTimes(int times)
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = true;
            route.CallCounter = times;

            //Act & Assert
            route.Verify(times);
        }

        [Fact]
        public void Verify_ShouldNotThrowException_WhenRouteIsVerifiableAndFuncReturnsTrue()
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = true;
            route.CallCounter = _fixture.Create<int>();

            //Act & Assert
            route.Verify(x => x > (route.CallCounter - 1));
        }

        [Fact]
        public void Verify_ShouldNotThrowException_WhenRouteIsVerifiableAndActionUsingMoqTimes()
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = true;
            route.CallCounter = _fixture.Create<int>();

            //Act & Assert
            route.Verify(x => Times.AtLeast(route.CallCounter - 1).Validate(x));
        }

        [Fact]
        public void Verify_ShouldThrowException_WhenRouteIsVerifiableCountNotEqual()
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = true;
            route.CallCounter = _fixture.Create<int>();

            //Act
            var exception = Assert.Throws<Exception>(() =>
                route.Verify(route.CallCounter + 1)
            );

            //Assert
            Assert.StartsWith("Route can not be verified", exception.Message);
        }

        [Fact]
        public void Verify_ShouldThrowException_WhenRouteIsVerifiableAndActionUsingMoqTimesNotTrue()
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = true;
            route.CallCounter = _fixture.Create<int>();

            //Act
            var exception = Assert.Throws<Exception>(() =>
                route.Verify(x => Times.AtLeast(route.CallCounter + 1).Validate(x))
            );

            //Assert
            Assert.Equal("Route can not be verified", exception.Message);
        }

        [Fact]
        public void Verify_ShouldThrowException_WhenRouteIsVerifiableAndActionReturnsFalse()
        {
            //Arrange
            var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
            route.IsVerifiable = true;
            route.CallCounter = _fixture.Create<int>();

            //Act
            var exception = Assert.Throws<Exception>(() => route.Verify(x => x < (route.CallCounter - 1)));

            //Assert
            Assert.Equal("Route can not be verified", exception.Message);
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
