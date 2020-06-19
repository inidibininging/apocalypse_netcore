using System;
using System.Runtime.Serialization;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Data
{
    [Serializable]
    public class GameStateNotFoundException : Exception
    {
        public GameStateNotFoundException()
        {
        }

        public GameStateNotFoundException(string message) : base(message)
        {
        }

        public GameStateNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GameStateNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}