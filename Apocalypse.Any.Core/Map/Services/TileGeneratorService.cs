//
//

using Apocalypse.Any.Core.Map.Flat;
using Apocalypse.Any.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Core.Map.Services
{
    /// <summary>
    /// This class generates from ugly bad code a map chunk
    /// </summary>
	public class TileGeneratorService
    {
        public TileGeneratorService()
        {
        }

        /// <summary>
        /// MapChunk generation and uglyness goes here ...
        /// </summary>
        /// <returns></returns>
        public MapChunk GenerateRandomMapChunk() //GraphicsDevice device)
        {
            var squareSize = Randomness.Instance.From(0, 4096);
            MapChunk chunk = new MapChunk(squareSize, squareSize);
            var layer = new Layer(Guid.NewGuid().ToString(), 3);

            //Yeah .. right.. hard code. The stuff that breaks every developers heart
            List<string> images = new List<string>()
            {
                //"BigMetalPlate00",
                //"BigMetalPlate01",
                //"BigMetalPlate02",
                //"BigMetalPlate03",
                //"ChipPlate00",
                //"ChipPlate01",
                //"ChipPlate02",
                //"ChipPlate03",
                //"ChipPlate04",
                "CablePlate00",
                "CablePlate01",
                "CablePlate02",
                "CablePlate03",
                "CablePlate04",
                "CablePlate05",
                //"Cthulu/PyramidAsset00",
                //"Cthulu/LandAsset00",
                //"Cthulu/LandAsset01",
                //"Cthulu/LandAsset02",
                // "Cthulu/TentacleAsset00",
                // "Cthulu/TentacleAsset01",
                // "Cthulu/TentacleAsset02",
                // "Cthulu/TentacleAsset03",
                // "Cthulu/TentacleAsset04",
                "CrystalPlate00",
                "MetalPlate00",
                "MetalPlate01"
            };

            int x = 0;
            int y = 0;

            var widthHeight = new Vector2(512, 512);

            //loops for loading a map chunk
            while (y + Convert.ToInt32(widthHeight.Y) < chunk.SizeY)
            {
                while (x + Convert.ToInt32(widthHeight.X) < chunk.SizeX)
                {
                    var draw = Randomness.Instance.From(0, 8);
                    if (draw > 5)
                    {
                        var someImageNr = Randomness.Instance.From(0, images.Count - 1);
                        var imageName = images.ElementAt(someImageNr);
                        var img = new MapImage(x, y, imageName);
                        if (imageName.ToLower().Contains("tentacle"))
                            img.CurrentImage.Color = Color.Green;
                        img.CurrentImage.Scale = new Vector2(2.5f, 2.5f); //TODO: Beware of this scale !!!
                        img.CurrentImage.LayerDepth = layer.LayerDepth;
                        layer.Tiles.Add(img);
                        //if(!(imageName == "ChipPlate00" || imageName == "ChipPlate01" || imageName == "ChipPlate02"))
                        //    layer.Tiles.Add(new MapImage(x, y, imageName));
                        //else
                        //    layer.Tiles.Add(new MapImage(x, y, imageName,
                        //        $"{imageName}_1",
                        //        $"{imageName}_2",
                        //        $"{imageName}_3"
                        //        ));
                    }
                    x += Convert.ToInt32(widthHeight.X);
                }
                y += Convert.ToInt32(widthHeight.Y);
                x = 0;
            }
            chunk.Layers.Add(layer);
            return chunk;
        }
    }
}