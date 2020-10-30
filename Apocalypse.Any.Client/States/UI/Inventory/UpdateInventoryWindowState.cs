using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Client.Services.Creation;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Drawing.UI;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;
using Apocalypse.Any.Domain.Common.Drawing.UI;

namespace Apocalypse.Any.Client.States.UI.Inventory
{
    public class UpdateInventoryWindowState : IState<string, INetworkGameScreen>
    {
        public const string InventorySlot = "slot";
        public const string Inventory = "inventory";

        public PlayerInventoryDrawingFactory InventoryFactory { get; set; }

        private ImageClient GetPlayerImage(IStateMachine<string, INetworkGameScreen> machine)
        {
            return machine.SharedContext.Images.Where(oldImg => oldImg.ServerData.Id == machine.SharedContext.PlayerImageId).FirstOrDefault();
        }
        public int Margin { get; set; } = 32;
        public int Offset { get; set; } = 64;
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(UpdateInventoryWindowState));

            //validate player
            var player = GetPlayerImage(machine);
            if (player == null)
            {
                machine.SharedContext.Messages.Add("player is not available");
                return;
            }

            if (player.Position == null)
            {
                machine.SharedContext.Messages.Add("player position is not available");
                return;
            }

            var inventoryWindow = machine.SharedContext.InventoryWindow;
            UpdateInventoryWindow(machine, inventoryWindow);
            UpdateInventorySlot(machine, inventoryWindow);
            UpdateInventoryImages(machine,inventoryWindow);

            machine.SharedContext.InventoryWindow.Update(machine.SharedContext.UpdateGameTime);
        }

        private void UpdateInventoryWindow(IStateMachine<string, INetworkGameScreen> machine, IWindow inventoryWindow)
        {
            var player = GetPlayerImage(machine);
            if (player == null)
            {
                machine.SharedContext.Messages.Add("player is not available");
                return;
            }

            if (player.Position == null)
            {
                machine.SharedContext.Messages.Add("player position is not available");
                return;
            }

            inventoryWindow.Position.X = (player.Position.X - 96); //(ScreenService.Instance.Resolution.X / 2) - 768);
            inventoryWindow.Position.Y = player.Position.Y + 128;
            
            //inventoryWindow.Position.X = MathHelper.Lerp(inventoryWindow.Position.X, machine.SharedContext.CursorImage.Position.X + inventoryWindow.Scale.X, 0.05f);
            //inventoryWindow.Position.Y = MathHelper.Lerp(inventoryWindow.Position.Y, machine.SharedContext.CursorImage.Position.Y , 0.05f);
            inventoryWindow.LayerDepth = DrawingPlainOrder.UI;
            inventoryWindow.Alpha.Alpha = 0.75f;
            inventoryWindow.IsVisible = true;
        }

        private void UpdateInventorySlot(IStateMachine<string, INetworkGameScreen> machine, IWindow inventoryWindow)
        {
            var player = GetPlayerImage(machine);
            if (player == null)
            {
                machine.SharedContext.Messages.Add("player is not available");
                return;
            }

            if (player.Position == null)
            {
                machine.SharedContext.Messages.Add("player position is not available");
                return;
            }

            foreach (var kv in inventoryWindow.Where(kv => kv.Key.Contains(InventorySlot)))
            {
                //validate gameobjectas a spritesheet
                var currentSlot = (kv.Value as SpriteSheet);
                if (currentSlot == null)
                    continue;
                currentSlot.LayerDepth = inventoryWindow.LayerDepth + DrawingPlainOrder.MicroPlainStep;
                currentSlot.Position.XDirection = inventoryWindow.Position.X + Margin;
                currentSlot.Position.YDirection = inventoryWindow.Position.Y + Margin;
            }
        }

        private void UpdateInventoryImages(IStateMachine<string, INetworkGameScreen> machine, IWindow inventoryWindow)
        {
            if (machine.SharedContext.InventoryImages == null)
            {
                if(machine.SharedContext.LastMetadataBag.Items != null)
                {
                    foreach(var item in machine.SharedContext.LastMetadataBag.Items)
                    {
                        
                        machine.SharedContext.Messages.Add(item.DisplayName);
                    }
                }
                //machine.SharedContext.Messages.Add("no inventory images found to update");
                return;
            }
        }
    }
}
