using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// Creates a bounding box around an IImageData
    /// </summary>
    public class BoundingBoxTransformationService
    {
        public Rectangle Transform(
            IImageData imageData,
            int offsetX,
            int offsetY) => new Rectangle
            (
                (int)Math.Round(imageData.Position.X - offsetX),
                (int)Math.Round(imageData.Position.X + offsetX),
                (int)Math.Round(imageData.Position.Y + offsetY),
                (int)Math.Round(imageData.Position.Y - offsetY)
                // (int)Math.Abs(imageData.Position.X - offsetX),
                // (int)Math.Abs(imageData.Position.X + offsetX),
                // (int)Math.Abs(imageData.Position.Y - offsetY),
                // (int)Math.Abs(imageData.Position.Y + offsetY)
            );
    }
}