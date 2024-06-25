using System;

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
