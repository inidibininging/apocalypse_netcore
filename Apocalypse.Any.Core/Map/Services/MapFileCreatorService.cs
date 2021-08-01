using Apocalypse.Any.Core.Map.Flat;
using Apocalypse.Any.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Apocalypse.Any.Core.Map.Services
{
    /// <summary>
    /// Dirty implementation for loading and saving map chunks as a compressed json string
    /// </summary>
    public class MapFileCreatorService
    {

        /// <summary>
        /// Loads a map chunk from a file path. This might crash if you load images that are no longer in the content pipe line.
        /// </summary>
        /// <param name="mapChunkFilePath">local file path</param>
        /// <returns></returns>
        public MapChunk MapChunkFromFile(string mapChunkFilePath)
        {
            var stringCompressorService = new StringCompressorService();

            var mapChunkFile = JsonConvert.DeserializeObject<MapChunkFile>(stringCompressorService.DecompressString(File.ReadAllText(mapChunkFilePath)));
            MapChunk mapChunk = new MapChunk(mapChunkFile.SizeX, mapChunkFile.SizeY);

            int maxY = mapChunkFile.SizeY;
            int maxX = mapChunkFile.SizeX;

            //ugly unoptimized for each b*tch code (cover your eyes)
            foreach (var layer in mapChunkFile.Layers)
            {
                var realLayer = new Layer(layer.Name, layer.Depth);
                for (int y = 0; y < maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        var imageLink = layer.Chunk[x, y];//mapChunkFile.Layers[layer.Name][x, y];
                        if (imageLink == 0)
                            continue;
                        realLayer.Tiles.Add(new MapImage(x, y, mapChunkFile.MappedUsedImages
                            [
                                layer.Chunk[x, y]
                            ]));
                    }
                }
                mapChunk.Layers.Add(realLayer);
            }
            return mapChunk;
        }

        /// <summary>
        /// Writes an instance of MapChunk as a file. The object is for now a json string compressed
        /// </summary>
        /// <param name="mapChunkFilePath"></param>
        /// <param name="mapChunk"></param>
        public void MapChunkToFile(string mapChunkFilePath, MapChunk mapChunk)
        {
            Dictionary<string, int> mappedUsedImages = new Dictionary<string, int>();
            //Dictionary<string, int[,]> rawMapChunk = new Dictionary<string, int[,]>();
            List<LayerFile> layerFiles = new List<LayerFile>();
            mappedUsedImages.Add(string.Empty, 0);
            int mappedUsedImageCursor = 1;

            //Spaghetti saving to ram for later saving it to a local file... -.-
            foreach (var layer in mapChunk.Layers)
            {
                int[,] currentMapChunk = new int[mapChunk.SizeX, mapChunk.SizeY];
                var layerFile = new LayerFile();
                layerFile.Name = layer.Name;
                layer.Tiles.ToList().ForEach(tile =>
                {
                    throw new NotImplementedException();
                    // var imgPath = tile.CurrentImage.Path.Replace(MapImage.BasePath, "");
                    // if (!mappedUsedImages.ContainsKey(imgPath))
                    // {
                    //     mappedUsedImages.Add(imgPath, mappedUsedImageCursor);
                    //     mappedUsedImageCursor += 1;
                    // }
                    // currentMapChunk
                    // [
                    //     Convert.ToInt32(tile.LocationUnit.X),
                    //     Convert.ToInt32(tile.LocationUnit.Y)
                    // ] = mappedUsedImages[imgPath];
                });
                layerFile.Depth = layer.LayerDepth;
                layerFile.Chunk = currentMapChunk;
                layerFiles.Add(layerFile);
            }
            MapChunkFile mapChunkFile = new MapChunkFile()
            {
                Layers = layerFiles,
                SizeX = mapChunk.SizeX,
                SizeY = mapChunk.SizeY,
                
                //TODO: this was a group by string. now all paths are integers.. so this class doesn't work anymore
                // MappedUsedImages = mappedUsedImages
                //     .GroupBy(p => p.Value)
                // .ToDictionary(g => g.Key, g => g.Select(pp => pp.Key).ToList().Single())
            };
            var stringCompressorService = new StringCompressorService();

            File.WriteAllText(mapChunkFilePath, stringCompressorService.CompressString(JsonConvert.SerializeObject(mapChunkFile)));
        }
    }
}