using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestMockCore
{
   public interface IHttpRequest
    {
        HttpMethod Method { get; set; }
        string Url { get; set; }

    }
}
