using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Exceptions
{
    public class ExecutionException : Exception
    {
        public ExecutionException()
            : base()
        {
        }
        public ExecutionException(string? _message)
            : base(_message)
        {
        }
    }
}
