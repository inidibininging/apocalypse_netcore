using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Drawing.UI;
using Apocalypse.Any.Core.Screen;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Text;
using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Domain.Common.Network;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Apocalypse.Any.Client.Screens
{
    public interface INetworkGameScreen : IGameScreen
    {
        string LoginToken { get; set; }
        string PlayerImageId { get; set; }

        #region UI

        SpriteSheet HealthImage { get; set; }
        SpriteSheet SpeedImage { get; set; }
        SpriteSheet StrenghImage { get; set; }
        SpriteSheet CursorImage { get; set; }
        // SpriteSheet LerpMouseImage { get; set; }

        #endregion UI

        ICharacterSheet FirstSheetSnapshot { get; set; }
        ICharacterSheet CurrentSheetSnapshot { get; set; }
        Atlas GameSheet { get; set; }
        GameTime UpdateGameTime { get; set; }
        GameClientConfiguration Configuration { get; set; }

        //string ServerIp { get; set; }
        //int ServerPort { get; set; }
        //UserData User { get; set; }
        List<string> Messages { get; set; }
        VisualText MultiplayerText { get; set; }
        List<ImageClient> Images { get; set; }
        List<ImageClient> InventoryImages { get; set; }
        int LoginTries { get; set; }
        int SecondsToNextLoginTry { get; set; }
        NetSendResult LoginSendResult { get; set; }
        NetClient Client { get; set; }
        IInputService InputService { get; set; }
        IdentifiableNetworkCommand CurrentNetworkCommand { get; set; }
        GameStateData CurrentGameStateData { get; set; }
        PlayerMetadataBag LastMetadataBag { get; set; }

        IWindow InfoWindow { get; set; }
        IWindow InventoryWindow { get; set; }
        IWindow CharacterWindow { get; set; }
        IWindow TradeWindow { get; set; }
        IWindow ChatWindow { get; set; }
    }
}