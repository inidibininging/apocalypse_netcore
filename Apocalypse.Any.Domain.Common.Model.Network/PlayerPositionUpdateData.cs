using System;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class PlayerPositionUpdateData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string LoginToken { get; set; }
        public int SectorKey { get; set; }
    }
}
