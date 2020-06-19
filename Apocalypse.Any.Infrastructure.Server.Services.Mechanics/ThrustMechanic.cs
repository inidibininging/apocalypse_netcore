using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class ThrustMechanic : IThrustMechanic
    //TODO: bind this => Behaviour<IFullPositionHolder>
    {
        public float BasicAcceleration { get; set; } = 1.5f;

        public ThrustMechanic()
        {
        }

        private void GetNextX(IFullPositionHolder target, float accelerationDelta)
        {
            var x = (float)(Math.Sin(target.Rotation));
            target.Position.X += x * BasicAcceleration * accelerationDelta;
            //target.Position.X = MathHelper.Lerp(target.Position.X, target.Position.X + (x * BasicAcceleration * accelerationDelta), 0.01f);
        }

        private void GetNextY(IFullPositionHolder target, float accelerationDelta)
        {
            var y = (float)(Math.Cos(target.Rotation)) * -1;
            target.Position.Y += y * BasicAcceleration * accelerationDelta;
            //target.Position.Y = MathHelper.Lerp(target.Position.Y, target.Position.Y + (y * BasicAcceleration * accelerationDelta), 0.01f);
        }

        public IFullPositionHolder Update(IFullPositionHolder target, float speedFactor = 1)
        {
            GetNextX(target, speedFactor);
            GetNextY(target, speedFactor);
            return target;
        }
    }
}