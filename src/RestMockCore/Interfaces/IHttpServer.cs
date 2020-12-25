using System;

namespace RestMockCore.Interfaces
{
    public interface IHttpServer : IDisposable
    {
        void Run();
        IRequestBuilder Config { get;  }
    }
}
