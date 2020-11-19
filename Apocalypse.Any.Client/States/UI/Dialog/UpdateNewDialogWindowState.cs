using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Text;
using Apocalypse.Any.Domain.Common.Drawing.UI;
using Apocalypse.Any.Domain.Common.Model.Network;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States.UI.Dialog
{
    public class UpdateNewDialogWindowState : IState<string, INetworkGameScreen>
    {
        private string LastDialogId { get; set; }
        private string LastDialogContent { get; set; }
        private List<Tuple<string, string>> LastDialogTree { get; set; }
        private ImageData LastDialogPortrait { get; set; }
        private Dictionary<string,int> LastDialogPositions { get; set; } = new Dictionary<string, int>();
        
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            var listBox = (machine
                .SharedContext
                .As<ApocalypseWindow>(BuildNewDialogWindowState.NewDialogWindow)
                .AllOfType<ApocalypseListBox<int>>()
                .FirstOrDefault());
            
            //this part happens when its either a new dialog or the dialog changed
            if (machine.SharedContext.LastMetadataBag.CurrentDialog?.Id != LastDialogId)
            {
                //remove the portrait
                if (LastDialogPortrait != null)
                {
                    machine.SharedContext.DialogWindow[LastDialogPortrait.Id].UnloadContent();
                    machine.SharedContext.DialogWindow.Remove(LastDialogPortrait.Id);
                }

                //remove title
                if (!string.IsNullOrWhiteSpace(LastDialogId))
                {
                    //clean head
                    machine.SharedContext.DialogWindow[LastDialogId].UnloadContent();
                    machine.SharedContext.DialogWindow.Remove(LastDialogId);
                }

                //remove all dialog trees
                if (LastDialogTree != null)
                {
                    //clean nodes
                    var dialogIds = LastDialogTree.Select(t => t.Item1);
                    var foundDialogIds = new List<string>();
                    foreach (var vt in machine.SharedContext.DialogWindow.Where(dialogWindowObject =>
                        dialogIds.Contains(dialogWindowObject.Key)))
                    {
                        foundDialogIds.Add(vt.Key);
                        vt.Value.UnloadContent();
                    }

                    foundDialogIds.ForEach(foundDialogId => machine.SharedContext.DialogWindow.Remove(foundDialogId));

                }

                //copy new dialog information
                LastDialogId = machine.SharedContext.LastMetadataBag.CurrentDialog?.Id;
                LastDialogContent = machine.SharedContext.LastMetadataBag.CurrentDialog?.Content;
                LastDialogTree = machine.SharedContext.LastMetadataBag.CurrentDialog?.DialogIdContent?.ToList();
                LastDialogPortrait = machine.SharedContext.LastMetadataBag.CurrentDialog?.Portrait;
                LastDialogPositions?.Clear();

                //create the dialog title
                if (LastDialogId != null)
                {
                    machine.SharedContext.DialogWindow.Add(LastDialogId, new VisualText());
                    machine.SharedContext.DialogWindow.As<VisualText>(LastDialogId).Text = LastDialogContent;
                    machine.SharedContext.DialogWindow.As<VisualText>(LastDialogId).Color = Color.Pink;
                    machine.SharedContext.DialogWindow.As<VisualText>(LastDialogId).Position.X =
                        machine.SharedContext.DialogWindow.Position.X + machine.SharedContext.DialogWindow.Scale.X;
                    machine.SharedContext.DialogWindow.As<VisualText>(LastDialogId).Position.Y =
                        machine.SharedContext.DialogWindow.Position.Y + machine.SharedContext.DialogWindow.Scale.Y;
                }

                //create the dialog tree
                if (LastDialogTree != null)
                {
                    var dialogOptionPosition = 1;

                    foreach (var newDialog in LastDialogTree)
                    {
                        machine.SharedContext.DialogWindow.Add(newDialog.Item1, new VisualText());
                        machine.SharedContext.DialogWindow.As<VisualText>(newDialog.Item1).Text = newDialog.Item2;
                        machine.SharedContext.DialogWindow.As<VisualText>(newDialog.Item1).Color = Color.Pink;
                        machine.SharedContext.DialogWindow.As<VisualText>(newDialog.Item1).Position.X =
                            machine.SharedContext.DialogWindow.Position.X + machine.SharedContext.DialogWindow.Scale.X;
                        machine.SharedContext.DialogWindow.As<VisualText>(newDialog.Item1).Position.Y =
                            machine.SharedContext.DialogWindow.Position.Y +
                            (machine.SharedContext.DialogWindow.Scale.Y *
                             dialogOptionPosition); //* machine.SharedContext.LastMetadataBag.CurrentDialog.FontSize);
                        dialogOptionPosition += 1;
                        LastDialogPositions.Add(newDialog.Item1, dialogOptionPosition);
                    }
                }
            }
        }
    }
}