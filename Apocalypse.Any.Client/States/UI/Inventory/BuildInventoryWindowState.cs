using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Drawing.UI;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Client.States.UI.Inventory
{
    public class BuildInventoryWindowState : IState<string, INetworkGameScreen>
    {
        private static Dictionary<string, Rectangle> InventoryHudSheet { get; set; }

        public BuildInventoryWindowState(Dictionary<string, Rectangle> gameSheet)
        {
            InventoryHudSheet = gameSheet;
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(BuildInventoryWindowState));
            machine.SharedContext.InventoryWindow = new ApocalypseWindow();
            machine.SharedContext.InventoryWindow.Position = new Core.Behaviour.MovementBehaviour();
            machine.SharedContext.InventoryWindow.Rotation = new Core.Behaviour.RotationBehaviour();
            machine.SharedContext.InventoryWindow.Color = Color.BlueViolet;

            //build nventory slots
            var rowCount = 6;
            var columnCount = 6;           

            var magicInventoryStringForNow = "inventory";

            var inventory = machine.SharedContext.Images.Where(img => img.ServerData.Id.Contains(magicInventoryStringForNow));
            if (inventory.Any())
            {
                machine.SharedContext.Messages.Add("inventory items found");
                rowCount = inventory.Count() / columnCount;
                if ((inventory.Count() % columnCount > 0))
                    rowCount += 1;
            }
            machine.SharedContext.InventoryWindow.Scale = new Vector2((columnCount * 32) + 32, (rowCount * 32) + 32);

            for (var indexX = 0; indexX < columnCount; indexX++)
            {
                for (var indexY = 0; indexY < rowCount; indexY++)
                {
                    var itemPosX = (indexX * 32);
                    var itemPosY = (indexY * 32);
                    var currentItemSprite = new SpriteSheet(InventoryHudSheet)
                    {
                        Path = "Image/hud_misc_edit",
                        SelectedFrame = "hud_misc_edit_0_0",
                        LayerDepth = DrawingPlainOrder.UI,
                        ForceDraw = true,
                        Position = new Core.Behaviour.MovementBehaviour()
                        {
                            X = machine.SharedContext.InventoryWindow.Position.X + itemPosX,
                            Y = machine.SharedContext.InventoryWindow.Position.Y + itemPosY
                        },
                        Color = Color.DarkViolet
                    };

                    //TODO:ScreenService. Hard coupled ContentManager. Fix this !!! This will be a bug!

                    //currentItemSprite.LoadContent(ScreenService.Instance.Content);

                    //TODO:way around needing the content manager and not having it is extending the Add function for all IGameObjects implementing IVisualGameObject.
                    //     This still shifts the dependency to the ueber owner of the object.
                    machine.SharedContext.InventoryWindow.Add(
                        $"{indexX}_{indexY}_slot",
                        currentItemSprite
                    );
                }
            }
        }
    }
}