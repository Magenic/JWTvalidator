using System;

namespace JwtValidator
{
    public class JsonWebTokenException : Exception
    {
        public JsonWebTokenException(string message)
            : base(message)
        {
        }
    }
}