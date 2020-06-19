using System;
using System.Runtime.Serialization;

namespace Apocalypse.Any.Domain.Common.Network
{
    [Serializable]
    public class NoLoginTokenException : Exception
    {
        public NoLoginTokenException()
        {
        }

        public NoLoginTokenException(string message) : base(message)
        {
        }

        public NoLoginTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoLoginTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}