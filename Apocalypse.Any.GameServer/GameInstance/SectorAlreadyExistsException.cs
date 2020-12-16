using System;
using System.Runtime.Serialization;

namespace Apocalypse.Any.GameServer.GameInstance
{
    [Serializable]
    internal class SectorAlreadyExistsException : Exception
    {
        public SectorAlreadyExistsException()
        {
        }

        public SectorAlreadyExistsException(int sectorId) : base($"Sector Id {sectorId} already exists")
        {
        }

        public SectorAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SectorAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}