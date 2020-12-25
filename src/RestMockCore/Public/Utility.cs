using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestMockCore
{
    public static class Utility
    {
        public static bool HasAny<T>(this IEnumerable<T> data)
        {
            return data != null && data.Any();
        }
    }
}
