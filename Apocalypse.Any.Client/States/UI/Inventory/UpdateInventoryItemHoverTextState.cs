using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Apocalypse.Any.Domain.Common.Drawing.UI;
using Apocalypse.Any.Domain.Common.Model.RPG;

namespace Apocalypse.Any.Client.States.UI.Inventory
{
    public class UpdateInventoryItemHoverTextState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.MultiplayerText == null)
                return;
            var itemWindowName = (machine.GetService.Get(nameof(BuildItemWindowState)) as BuildItemWindowState)?.ItemWindowName ?? throw new Exception($"Cannot find {nameof(BuildItemWindowState)} or ItemWindowName is not defined");
            if (!machine.SharedContext.ContainsKey(itemWindowName))
                return;
            
            machine.SharedContext.MultiplayerText.Text = "";
            machine.SharedContext.MultiplayerText.Color = Color.Purple;
            machine.SharedContext.MultiplayerText.LayerDepth = DrawingPlainOrder.UIFX;

            var cursorImageAsVector = machine.SharedContext.CursorImage.Position.ToVector2();
            var hoveredItemImage= machine.SharedContext.InventoryImages.FirstOrDefault(inventoryItemImage => Vector2.Distance(cursorImageAsVector, inventoryItemImage.Position.ToVector2()) < 16);
            var itemWindow = machine.SharedContext.As<ApocalypseWindow>(itemWindowName);
            if (hoveredItemImage == null)
            {
                itemWindow.IsVisible = false;
                return;
            }

            if (machine.SharedContext.LastMetadataBag == null)
                return;

            //TODO: possible bug if current image of items changes.. there will be a flickering effect on the item info text
            machine.SharedContext.LastMetadataBag.Items.ForEach(itm => itm.CurrentImage.Scale = new Vector2(1));
            var itemData = machine.SharedContext.LastMetadataBag.Items.FirstOrDefault(item => item.CurrentImage.Id == hoveredItemImage.ServerData.Id);
            
            if (itemData == null)
            {
                itemWindow.IsVisible = false;
                return;
            }
    
            hoveredItemImage.Scale = new Vector2(hoveredItemImage.Scale.X + 0.5f, hoveredItemImage.Scale.Y + 0.5f);
            
            /*TODO: For now, the item window will only have a single list box containing the stats information.
                    If more information is needed, the list box needs a known name. 
                    This can be stored in the BuildItemWindowState */
            var statsListBox = itemWindow.AllOfType<ApocalypseListBox<int>>().FirstOrDefault();
            foreach (var itemStatsProperty in itemData
                .Stats
                .GetType()
                .GetProperties()
                .Where(p => p.CanRead)
                .Select(p => p))
            {
                var statValue = itemStatsProperty.GetValue(itemData.Stats);
                var statName = itemStatsProperty.Name;
                statsListBox.As<ApocalypseListItem<int>>(statName).Text = $"{statName}:{statValue}";
            }
            itemWindow.Position.X = cursorImageAsVector.X + 128; 
            itemWindow.Position.Y = cursorImageAsVector.Y;
            itemWindow.LayerDepth = machine.SharedContext.InventoryWindow.LayerDepth + (DrawingPlainOrder.MicroPlainStep * 2);

            var wtf = (statsListBox.Items.OrderBy(listItem => listItem.Position.Y).FirstOrDefault().Position.Y - statsListBox.Position.Y); // The list is 
            var itemWindowSize = statsListBox
                                            .Items
                                            .Select(i => (
                                                (i.Scale.Y * i.Height) + // list item sizes 
                                                MathF.Abs(statsListBox.SpaceBetweenListItem) // the space between two list items
                                                )).Sum() +  wtf
                                 ; // the space between window and the first listbox item 
            itemWindow.Scale = new Vector2(itemWindow.Scale.X, itemWindowSize); //The 16 is a little breathing space
            
            itemWindow.IsVisible = true;


            var spacing = machine.SharedContext.MultiplayerText.Text.Split(Environment.NewLine).Length;
            machine.SharedContext.MultiplayerText.Position.X = cursorImageAsVector.X;
            machine.SharedContext.MultiplayerText.Position.Y = cursorImageAsVector.Y + spacing;
        }
    }
}
