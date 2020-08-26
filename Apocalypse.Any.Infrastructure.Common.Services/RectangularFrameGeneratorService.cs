using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Common.Services
{
    public class RectangularFrameGeneratorService : IRectangularFrameGeneratorService
    {
        public Dictionary<string, Rectangle> GenerateGameSheetAtlas
        (
            string prefixName = "frame",
            int frameSizeX = 32,
            int frameSizeY = 32,
            int startRangeX = 0,
            int endRangeX = 1,
            int startRangeY = 0,
            int endRangeY = 1)
        {
            Dictionary<string, Rectangle> finalAtlas = new Dictionary<string, Rectangle>();
            for (var currentX = startRangeX; currentX <= endRangeX; currentX++)
            {
                for (int currentY = startRangeY; currentY <= endRangeY; currentY++)
                {
                    finalAtlas.Add($"{prefixName}_{currentX}_{currentY}", new Rectangle(currentX * frameSizeX,
                                                                                currentY * frameSizeY,
                                                                                frameSizeX,
                                                                                frameSizeY));
                }
            }
            return finalAtlas;
        }
    }
}