using System;

namespace Apocalypse.Any.Core.Map.Flat
{
    [Serializable]
    public class LayerFile
    {
        public string Name { get; set; }
        public int Depth { get; set; }
        public int[,] Chunk { get; set; }
    }
}