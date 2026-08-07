using Xunit;

namespace RestMockCore.Test;

/// <summary>
/// All tests that spin up a real <c>HttpServer</c> share a hardcoded port
/// (see <c>HttpServerTests.PORT</c>), so they must run serially.
/// </summary>
[CollectionDefinition(Name)]
public class HttpServerCollection
{
    public const string Name = "HttpServerCollection";
}
