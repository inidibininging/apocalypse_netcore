using System;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class AttractionMechanic : IAttractionMechanic
    {
        public float DistanceTrigger { get; set; } = 4096f;
        public void Update(IFullPositionHolder target, IFullPositionHolder attractionPoint, float force )
        {
            var dist = Vector2.Distance(target.Position.ToVector2(),
                                        attractionPoint.Position.ToVector2());
            if(dist > DistanceTrigger)
                return;

            target.Position.X = MathHelper.Lerp(
                target.Position.X,
                attractionPoint.Position.X,
                force);
                
            // target.Rotation.Rotation = MathHelper.Lerp(target.Rotation.Rotation, attractionPoint.Rotation.Rotation - 180, force);

            target.Position.Y = MathHelper.Lerp(
                    target.Position.Y,
                    attractionPoint.Position.Y,
                    force);

        }
    }
}