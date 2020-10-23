using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class Atlas
    {
        public string Name { get; set; }
        public Dictionary<(int frame, int x, int y), Rectangle> Frames { get; set; }
    }
}