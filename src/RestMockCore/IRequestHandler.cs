using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestMockCore
{


    public interface IRequestHandler
    {
        HttpResponse Response { get; }
        void Send(string body);
        void Send(string body, int statusCode);
        void Send(string body, int statusCode, Dictionary<string, string> headers);

    }


}
