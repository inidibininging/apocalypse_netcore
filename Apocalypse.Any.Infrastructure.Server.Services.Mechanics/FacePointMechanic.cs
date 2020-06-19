using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class FacePointMechanic : IFacePointMechanic
    {
        private const float V = ((float)(180 / MathF.PI));

        public void Update(IFullPositionHolder target, IFullPositionHolder attractionPoint)
        {
            var targetVector = attractionPoint.Position.ToVector2() - target.Position.ToVector2();
            target.Rotation.Rotation = MathF.Atan2(targetVector.Y, targetVector.X) * V;
        }
    }
}