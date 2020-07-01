using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations
{
    public class ImageToRectangleTransformationService
    {
        public Rectangle Transform(IImageData image, int quadracticOffset = 0)
        {
	    var realWidth = image.Width * image.Scale.X;
	    var realHeight = image.Height * image.Scale.Y;
            return new Rectangle(
                                    (int)MathF.Round(image.Position.X) - (int)MathF.Round(realWidth/2),
                                    (int)MathF.Round(image.Position.Y) - (int)MathF.Round(realHeight/2),
                                    (int)MathF.Round(realWidth * 2f) + quadracticOffset,
                                    (int)MathF.Round(realHeight * 2f) + quadracticOffset
                                );
        }
    }
}
