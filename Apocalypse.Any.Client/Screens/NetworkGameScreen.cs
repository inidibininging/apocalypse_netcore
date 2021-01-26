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
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Domain.Common.Drawing.UI;

namespace Apocalypse.Any.Client.Screens
{
    public class NetworkGameScreen : GameScreen, INetworkGameScreen
    {
        public VisualText MultiplayerText { get; set; } = new VisualText();
        public List<ImageClient> Images { get; set; } = new List<ImageClient>();
        public List<ImageClient> InventoryImages { get; set; } = new List<ImageClient>();
        public IInputService InputService { get; set; }

        public NetClient Client { get; set; }
        public NetSendResult LoginSendResult { get; set; }
        public string LoginToken { get; set; }
        public string PlayerImageId { get; set; }

        #region UI

        public SpriteSheet HealthImage { get; set; } = new SpriteSheet(null) { Path = ImagePaths.hud_misc_edit };
        public SpriteSheet SpeedImage { get; set; } = new SpriteSheet(null) { Path = ImagePaths.hud_misc_edit };
        public SpriteSheet StrenghImage { get; set; } = new SpriteSheet(null) { Path = ImagePaths.hud_misc_edit };       
        public SpriteSheet DialogImage { get; set; } = new SpriteSheet(null) { Path = ImagePaths.hud_misc_edit };
         

        //public SpriteSheet LerpMouseImage { get; set; } = new SpriteSheet(null) { Path = "Image/hud_misc" };

        public ICharacterSheet FirstSheetSnapshot { get; set; }
        public ICharacterSheet CurrentSheetSnapshot { get; set; }


        public VisualText MoneyCount { get; set; }
        public IWindow InfoWindow { get; set; }
        public IWindow InventoryWindow { get; set; }
        public IWindow CharacterWindow { get; set; }
        public IWindow TradeWindow { get; set; }
        public IWindow ChatWindow { get; set; }
        public IWindow DialogWindow { get; set; }

        #endregion UI

        #region Atlas Stuff
        public GameClientConfiguration Configuration { get; set; }
        public Atlas GameSheet { get; set; } = new Atlas()
        {
            Name = "gamesheetExtended",
            Frames = new Dictionary<(int frame,int x, int y), Rectangle>()
        };

        //public string ServerIp { get; set; }
        //public int ServerPort { get; set; }
        //public UserData User { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public int LoginTries { get; set; } = 0;
        public int SecondsToNextLoginTry { get; set; } = 1;
        public GameTime UpdateGameTime { get; set; }
        public IdentifiableNetworkCommand CurrentNetworkCommand { get; set; }
        public GameStateData CurrentGameStateData { get; set; }
        public PlayerMetadataBag LastMetadataBag { get; set; }

        #endregion Atlas Stuff

        public NetworkGameScreen()
        {
            CursorImage = new SpriteSheet(null) { Path = ImagePaths.hud_misc_edit  };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            ScreenService.Instance.GraphicsDevice.Viewport = ScreenService.Instance.DefaultScreenCamera.CurrentViewport;
            spriteBatch.Begin(
                SpriteSortMode.FrontToBack, // this is also the order for building the screen.
                null,
                SamplerState.PointClamp,
                null,
                null,
                null,
                ScreenService.Instance.DefaultScreenCamera.TransformMatrix);
            base.Draw(spriteBatch);

            if (!Images.Any())
            {
                Messages.Add("no image received from server");
            }
            foreach (var img in Images.ToList())
                img.Draw(spriteBatch);

            if (!InventoryImages.Any())
            {
                Messages.Add("no inventory image received from server");
            }
            foreach (var img in InventoryImages.ToList())
                img.Draw(spriteBatch);

            MultiplayerText.Draw(spriteBatch);
            HealthImage.Draw(spriteBatch);
            SpeedImage.Draw(spriteBatch);
            StrenghImage.Draw(spriteBatch);
            DialogImage.Draw(spriteBatch);
            CursorImage.Draw(spriteBatch);
            
            //LerpMouseImage.Draw(spriteBatch);

            if (InfoWindow?.IsVisible == true)
                InfoWindow?.Draw(spriteBatch);

            if (DialogWindow?.IsVisible == true)
                DialogWindow?.Draw(spriteBatch);

            if (InventoryWindow?.IsVisible == true)
                InventoryWindow.Draw(spriteBatch);

            if (CharacterWindow?.IsVisible == true)
            {
                MoneyCount.Draw(spriteBatch);
                CharacterWindow?.Draw(spriteBatch);
            }

            if (CharacterWindow?.IsVisible == true)
                CharacterWindow?.Draw(spriteBatch);

            if (ChatWindow?.IsVisible == true)
                ChatWindow?.Draw(spriteBatch);

            spriteBatch.End();
        }

        public override void Update(GameTime time)
        {
            UpdateGameTime = time;
            Messages.Add("Updated time");
            InputService.Update(time);
            //MultiplayerText.Text = string.Join(System.Environment.NewLine,Messages);
            
            
            base.Update(time);
        }
    }
}
