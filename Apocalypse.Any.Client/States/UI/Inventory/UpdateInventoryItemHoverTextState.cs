using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Client.States.UI.Inventory
{
    public class UpdateInventoryItemHoverTextState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.MultiplayerText == null)
                return;
            machine.SharedContext.MultiplayerText.Text = "";
            machine.SharedContext.MultiplayerText.Color = Color.Purple;
            machine.SharedContext.MultiplayerText.LayerDepth = DrawingPlainOrder.UIFX;

            var cursorImageAsVector = machine.SharedContext.CursorImage.Position.ToVector2();
            var hoveredItemImage= machine.SharedContext.InventoryImages.FirstOrDefault(inventoryItemImage => Vector2.Distance(cursorImageAsVector, inventoryItemImage.Position.ToVector2()) < 16);
            
            if (hoveredItemImage == null)
                return;

            if (machine.SharedContext.LastMetadataBag == null)
                return;

            //TODO: possible bug if current image of items changes.. there will be a flickering effect on the item info text

            machine.SharedContext.LastMetadataBag.Items.ForEach(itm => itm.CurrentImage.Scale = new Vector2(1));

            var itemData = machine.SharedContext.LastMetadataBag.Items.FirstOrDefault(item => item.CurrentImage.Id == hoveredItemImage.ServerData.Id);
            if (itemData == null)
                return;
            hoveredItemImage.Scale = new Vector2(hoveredItemImage.Scale.X + 0.5f, hoveredItemImage.Scale.Y + 0.5f);
            machine.SharedContext.MultiplayerText.Text = $@"{itemData.DisplayName}
ATK: {itemData.Stats.Attack}
DEF: {itemData.Stats.Defense}
STR: {itemData.Stats.Strength}
SPD: {itemData.Stats.Speed}
TEC: {itemData.Stats.Technology}
AUR: {itemData.Stats.Aura}
CHR: {itemData.Stats.Charisma}";

            var spacing = machine.SharedContext.MultiplayerText.Text.Split(Environment.NewLine).Length;

            machine.SharedContext.MultiplayerText.Position.X = cursorImageAsVector.X;
            machine.SharedContext.MultiplayerText.Position.Y = cursorImageAsVector.Y + spacing;
        }
    }
}
