using Apocalypse.Any.Core.Model;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class GameStateData : IIdentifiableModel
    {
        public string Id { get; set; }
        public string LoginToken { get; set; }

        public IdentifiableNetworkCommand Metadata { get; set; }

        public CameraData Camera { get; set; }
        public ScreenData Screen { get; set; }

        public List<string> Commands { get; set; }
        public List<string> Sounds { get; set; }
        public List<ImageData> Images { get; set; }
    }
}