using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations
{
    public class ImageToRectangleTransformationService
    {
        public Rectangle Transform(IImageData image, int quadracticOffset = 0)
        {
            return new Rectangle(
                                    (int)MathF.Round(image.Position.X) - (int)MathF.Round(image.Width/2),
                                    (int)MathF.Round(image.Position.Y) - (int)MathF.Round(image.Height/2),
                                    (int)MathF.Round(image.Width  * 2f) + quadracticOffset,
                                    (int)MathF.Round(image.Height * 2f) + quadracticOffset
                                );
        }
    }
}