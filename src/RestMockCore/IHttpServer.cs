using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestMockCore
{
    public interface IHttpServer : IDisposable
    {
        void Run();
        IRequestBuilder Config { get;  }
    }
}
