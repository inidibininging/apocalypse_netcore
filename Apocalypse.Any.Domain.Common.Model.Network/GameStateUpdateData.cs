using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class GameStateUpdateData
    {
        public string LoginToken { get; set; }
        public ScreenData Screen { get; set; }
        public List<string> Commands { get; set; }
    }
}