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
            // var rightBound = attractionPoint.Position.X + DistanceTrigger/2;
            // var leftBound = attractionPoint.Position.X - DistanceTrigger/2;

            // var upBound = attractionPoint.Position.Y - DistanceTrigger/2;
            // var downBound = attractionPoint.Position.Y + DistanceTrigger/2;


            // if (rightBound < target.Position.X)


            // if (leftBound > target.Position.X)
            //     target.Position.X += force;

            // if(upBound > target.Position.Y)
            //     target.Position.Y += force;


            // var attractionRectangle = new Rectangle(
            //                     Convert.ToInt32(MathF.Ceiling(attractionPoint.Position.X)) - (int)(attractionOffset / 2),
            //                     Convert.ToInt32(MathF.Ceiling(attractionPoint.Position.Y)) - (int)(attractionOffset / 2),
            //                     (int)(attractionOffset * 2),
            //                     (int)(attractionOffset * 2)
            //                     );

            // var targetRectangle = new Rectangle(
            //                     Convert.ToInt32(MathF.Ceiling(target.Position.X)) - (int)(attractionOffset / 2),
            //                     Convert.ToInt32(MathF.Ceiling(target.Position.Y)) - (int)(attractionOffset / 2),
            //                     (int)(attractionOffset * 2),
            //                     (int)(attractionOffset * 2)
            //                     );

            // Rectangle outputRectangle = Rectangle.Empty;

            // if (attractionPoint.Position.X - attractionOffset > target.Position.X)
            //     target.Position.X += force;

            // if (attractionPoint.Position.X + attractionOffset < target.Position.X)
            //     target.Position.X -= force;

            // if (attractionPoint.Position.Y - attractionOffset > target.Position.Y)
            //     target.Position.Y += force;

            // if (attractionPoint.Position.Y + attractionOffset < target.Position.Y)
            //     target.Position.Y -= force;

            // foreach( var target in targetsToAttract )
            // {
            // var attractionLength = attractionPoint.Position.ToVector2().Length();

            // var targetLength = target.Position.ToVector2().Length();

            // var dotProduct = Vector2.Dot(
            //                             attractionPoint.Position.ToVector2(),
            //                             attractionPoint.Position.ToVector2())
            //                             / (attractionLength * targetLength
            //                             );
            // var inverseCos = Math.Acos(dotProduct);

            // if(inverseCos != double.NaN){
            //     Console.WriteLine(inverseCos* (180 / Math.PI));
            //     if(inverseCos < float.MaxValue)
            //         target.Rotation.Rotation -= (float)inverseCos  * (float)(180 / Math.PI);
            // }

            //attraction point is west from target
            // if (attractionPoint.Position.X - attractionOffset < target.Position.X)
            //     target.Position.X += force;
            // else
            //     target.Position.X -= force;

            //attraction point is east from target

            // if(attractionPoint.Position.X + attractionOffset > target.Position.X)
            //     continue;
            // else
            //     target.Position.X -= force;

            //attraction point is south from target
            // if (attractionPoint.Position.Y - attractionOffset < target.Position.Y)
            //     target.Position.Y += force;
            // else
            //     target.Position.Y -= force;

            //attraction point is north from target
            // if(attractionPoint.Position.Y + attractionOffset > target.Position.Y)
            //     continue;
            // else
            //     target.Position.Y -= force;

            //var vectorsMultiplied = attractionPoint.Position.ToVector2() * target.Position.ToVector2();

            // }
        }
    }
}