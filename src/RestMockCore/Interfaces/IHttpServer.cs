using System;

namespace RestMockCore
{
    public interface IHttpServer : IDisposable
    {
        void Run();
        IRequestBuilder Config { get;  }
    }
}
