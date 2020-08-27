using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public struct DeltaGameStateData
    {
        public string Id { get; set; }
        public string LoginToken { get; set; }
        public float? CameraX { get; set; }
        public float? CameraY { get; set; }
        public float? CameraRotation { get; set; }

        public int? ScreenWidth { get; set; }
        public int? ScreenHeight { get; set; }

        public List<string> Commands { get; set; }
        public List<int> Sounds { get; set; }
        public List<DeltaImageData> Images { get; set; }
    }
}
