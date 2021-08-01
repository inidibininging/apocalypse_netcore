using System.Collections.Generic;
using Apocalypse.Any.Constants;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Client.Services
{
    public class RectangularFrameGeneratorService
    {
        public Dictionary<(int frame, int x, int y), Rectangle> GenerateGameSheetAtlas
        (
            int prefixName = ImagePaths.DefaultFrame,
            int frameSizeX = 32,
            int frameSizeY = 32,
            int startRangeX = 0,
            int endRangeX = 1,
            int startRangeY = 0,
            int endRangeY = 1)
        {
            var finalAtlas = new Dictionary<(int frame, int x, int y), Rectangle>();
            for (var currentX = startRangeX; currentX <= endRangeX; currentX++)
            for (var currentY = startRangeY; currentY <= endRangeY; currentY++)
                finalAtlas.Add((prefixName, currentX, currentY), new Rectangle(currentX * frameSizeX,
                    currentY * frameSizeY,
                    frameSizeX,
                    frameSizeY));


            return finalAtlas;
        }
    }
}