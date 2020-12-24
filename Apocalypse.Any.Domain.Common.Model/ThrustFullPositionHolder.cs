using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class ThrustFullPositionHolder : IFullPositionHolder
    {
        public MovementBehaviour Position { get; set; }
        public RotationBehaviour Rotation { get; set; }
        public float SpeedFactor { get; set; }
    }
}