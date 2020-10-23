using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Map.Flat
{
    [Serializable]
    public class MapChunkFile
    {
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public Dictionary<int, int> MappedUsedImages { get; set; }
        public List<LayerFile> Layers { get; set; }
    }
}