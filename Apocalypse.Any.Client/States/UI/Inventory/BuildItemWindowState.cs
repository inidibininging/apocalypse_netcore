using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Domain.Common.Drawing.UI;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States.UI.Inventory
{
    /// <summary>
    /// Creates a window with all the stats information (see class CharacterSheet) of an item as list items.
    /// </summary>
    public class BuildItemWindowState : IState<string, INetworkGameScreen>
    {
        public Dictionary<string, (int frameName,int x, int y)> ItemFrames { get; }  
         = new Dictionary<string, (int frameName, int x, int y)>()
         {
             {"Default", (ImagePaths.HUDFrame, 0, 0)},
             {"Attack", (ImagePaths.HUDFrame, 0, 3)},
             {"Defense", (ImagePaths.HUDFrame, 5, 0)},
             {"Health", (ImagePaths.HUDFrame, 6, 0)},
             {"Strength", (ImagePaths.HUDFrame, 4, 0)},
             {"Technology", (ImagePaths.HUDFrame, 8, 0)},
             {"Speed", (ImagePaths.HUDFrame, 1, 1)},
             {"Aura", (ImagePaths.HUDFrame, 0, 1)},
             {"Charisma", (ImagePaths.HUDFrame, 0, 2)},
             {"Experience", (ImagePaths.HUDFrame, 1, 0)},
             {"Kills", (ImagePaths.HUDFrame, 7, 2)},
         };
        public string ItemWindowName { get; } = "ItemWindow";

        public BuildItemWindowState(Dictionary<(int frame, int x, int y), Rectangle> itemFrames)
        {
            // ItemFrames = itemFrames ?? throw new ArgumentNullException(nameof(itemFrames));
        }
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.ContainsKey(ItemWindowName)) return;
            var itemWindow = new ApocalypseWindow();
            itemWindow.Color = Color.DarkViolet;
            
            var statsListBox = new ApocalypseListBox<int>(machine.SharedContext.GameSheet.Frames.Where(frameKV => frameKV.Key.frame == ImagePaths.HUDFrame).ToDictionary(k => k.Key, v => v.Value));
            
            foreach (var statName in typeof(CharacterSheet)
                .GetProperties()
                .Where(p => p.CanRead)
                .Select(p => p.Name))
                statsListBox.Add(statName, ItemFrames[statName], useTextAsKey: true);
            itemWindow.Add(statsListBox);
            //Adjust window size to the size of all list items 
            
            itemWindow.IsVisible = false;
            machine.SharedContext.Add(ItemWindowName, itemWindow);
        }
    }
}