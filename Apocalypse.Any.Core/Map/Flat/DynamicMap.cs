using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Map.Flat
{
    public class DynamicMap : GameObject
    {
        public List<MapChunk> Chunks { get; set; }

        public DynamicMap()
        {
        }

        public override void Update(GameTime time)
        {
        }
    }
}