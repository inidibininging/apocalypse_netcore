using Apocalypse.Any.Core.Behaviour;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class CameraData
    {
        public MovementBehaviour Position { get; set; }
        public RotationBehaviour Rotation { get; set; }
    }
}