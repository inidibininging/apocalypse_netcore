using Apocalypse.Any.Core.Model;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class GameStateData : IIdentifiableModel
    {
        public string Id { get; set; }
        
         
        public string LoginToken { get; set; }
        
        /// <summary>
        /// Used for transmitting any information via network command
        /// </summary>
        public IdentifiableNetworkCommand Metadata { get; set; }

        
        public CameraData Camera { get; set; }
        
        /// <summary>
        /// Size of the actual game window
        /// </summary>
        public ScreenData Screen { get; set; }

        /// <summary>
        /// List of commands the player executes on the client side
        /// </summary>
        public List<string> Commands { get; set; }
        
        // public List<string> Sounds { get; set; }
        
        public List<ImageData> Images { get; set; }


    }
}