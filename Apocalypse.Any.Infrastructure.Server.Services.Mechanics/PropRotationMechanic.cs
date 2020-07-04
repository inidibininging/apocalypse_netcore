using System;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class PropRotationMechanic : ISingleFullPositionHolderMechanic<ImageData>
    {
        private readonly bool useRight = Randomness.Instance.From(0,100) > 50;
        private readonly float randomRotation = Randomness.Instance.From(0,100)/1500f;
        
        public ImageData Update(ImageData entity)
        {
            if (!entity.SelectedFrame.Contains("planet") && !entity.SelectedFrame.Contains("fog"))
                return entity;
            // var x = entity.Width*entity.Scale.X;
            // var y = entity.Height*entity.Scale.Y;
            // var circum = (System.MathF.Pow(x,2) + System.MathF.Pow(y,2)) / 2;
            // var rotationSpeed = circum/(x*y);
            //Console.WriteLine(randomRotation);

            if(entity.Rotation.Rotation > 360)
                entity.Rotation.Rotation = 0;

            //separation rotation speed for props            
            var finalRotation = randomRotation;
            if(entity.SelectedFrame.Contains("planet"))
                finalRotation /= 4;
            if(entity.SelectedFrame.Contains("fog"))
                finalRotation /= 6;

            if (useRight)
            {
                entity.Rotation.Rotation += finalRotation;
            }
            else
            {
                entity.Rotation.Rotation -= finalRotation;
            }

            return entity;
        }
    }
}
