using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Client.Mechanics
{
    public class ZeroThrustMechanicBehaviour : ThrustMechanicBehaviour<IImage>
    {
        public ZeroThrustMechanicBehaviour(IImage target) : base(target)
        {
        }

        protected override float GetDefaultAcceleration()
        {
            return 0.0f;
        }
    }
}