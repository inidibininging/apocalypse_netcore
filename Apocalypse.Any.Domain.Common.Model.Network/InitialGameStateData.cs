using Apocalypse.Any.Core.Model;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class InitialGameStateData : IIdentifiableModel
    {
        public string Id { get; set; }
        public Dictionary<string, int> SoundsToIntegerMaps { get; set; }
        public Dictionary<string, int> ImageToIntegerMaps { get; set; }
        public Dictionary<int, Dictionary<string, Rectangle>> SpriteSheetIntegerMaps { get; set; }
        public Dictionary<string, Dictionary<string, Rectangle>> SpriteSheetStringMaps { get; set; }
    }
}