using System.Collections.Generic;

namespace Apocalypse.Any.Core.Map.Flat
{
    public class Layer
    {
        #region properties

        private List<MapImage> _tiles;

        public List<MapImage> Tiles
        {
            get
            {
                return _tiles ?? (_tiles = new List<MapImage>());
            }
            internal set { _tiles = value; }
        }

        public string Name { get; private set; }

        public int LayerDepth
        {
            get;
            private set;
        }

        #endregion properties

        public Layer(string name, int depth = 3)
        {
            Name = name;
            LayerDepth = depth;
        }
    }
}