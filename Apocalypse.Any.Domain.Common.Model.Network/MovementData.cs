using Apocalypse.Any.Core.Model;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class MovementData : IIdentifiableModel
    {
        public string Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int RotationAngle { get; set; }
    }
}