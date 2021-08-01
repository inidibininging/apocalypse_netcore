using System;
using Apocalypse.Any.Constants;
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
        public bool Active { get; set; } = true;

        private bool IsPlanetFrame(int frame) => frame == ImagePaths.PlanetFrame ||
                                                frame == ImagePaths.RandomPlanetFrame0 ||
                                                frame == ImagePaths.RandomPlanetFrame1 ||
                                                frame == ImagePaths.RandomPlanetFrame2 ||
                                                frame == ImagePaths.RandomPlanetFrame3;
        public ImageData Update(ImageData entity)
        {
            if (!IsPlanetFrame(entity.SelectedFrame.frame) && entity.SelectedFrame.frame != ImagePaths.FogFrame)
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
            if(IsPlanetFrame(entity.SelectedFrame.frame))
                finalRotation /= 4;
            if(entity.SelectedFrame.frame == ImagePaths.FogFrame)
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
