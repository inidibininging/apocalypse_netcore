using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Common.Services
{
    public interface IRectangularFrameGeneratorService
    {
        Dictionary<string, Rectangle> GenerateGameSheetAtlas(string prefixName = "frame", int frameSizeX = 32, int frameSizeY = 32, int startRangeX = 0, int endRangeX = 1, int startRangeY = 0, int endRangeY = 1);
    }
}