using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class Atlas
    {
        public string Name { get; set; }
        public Dictionary<string, Rectangle> Frames { get; set; }
    }
}