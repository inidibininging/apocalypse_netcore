using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Text;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Client.States.UI.Dialog
{
    public class UpdateDialogHoverTextState : IState<string, INetworkGameScreen>
    {
        
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            var cursorImageAsVector = machine.SharedContext.CursorImage.Position.ToVector2();
            var hoveredText = machine.SharedContext.DialogWindow.Values.OfType<VisualText>().FirstOrDefault(inventoryItemImage => Vector2.Distance(cursorImageAsVector, inventoryItemImage.Position) < 32);
            
            foreach (var txt in machine.SharedContext.DialogWindow.Values.OfType<VisualText>())
                txt.Scale = new Vector2(1.0f);

            if (hoveredText == null)
                return;

            var dialog = machine.SharedContext.LastMetadataBag?.CurrentDialog.DialogIdContent.FirstOrDefault(dlg => dlg.Item2 == hoveredText.Text);
            if(dialog == null)
            {
                
            }
            machine.SharedContext.LastMetadataBag.ClientEventName = hoveredText.Text;
            hoveredText.Scale = new Vector2(hoveredText.Scale.X + 0.25f, hoveredText.Scale.Y + 0.25f);
        }
    }
}
