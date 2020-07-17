using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Text;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Client.States.UI.Dialog
{
    public class UpdateDialogWindowState : IState<string, INetworkGameScreen>
    {
        public string LastDialogId { get; set; }
        public string LastDialogContent { get; set; }
        private List<Tuple<string, string>> LastDialogTree { get; set; }
        private ImageData LastDialogPortrait { get; set; }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.DialogWindow == null)
                return;

            if (machine.SharedContext.LastMetadataBag == null)
            {
                machine.SharedContext.DialogWindow.IsVisible = false;
                return;
            }
            var txtRow = 1;
            if (machine.SharedContext.DialogWindow.IsVisible)
            {
                var playerImage = machine.SharedContext.Images.FirstOrDefault(img => img.ServerData.Id == machine.SharedContext.PlayerImageId);
                machine.SharedContext.DialogWindow.Position.X = playerImage.Position.X - 256;
                machine.SharedContext.DialogWindow.Position.Y = playerImage.Position.Y - 486;
                machine.SharedContext.DialogWindow.LayerDepth = DrawingPlainOrder.UI - 0.1f;
                machine.SharedContext.DialogWindow.Alpha.Alpha = 0.5f;

                foreach (var text in machine.SharedContext.DialogWindow.Where(txts => txts.Value.GetType() == typeof(VisualText)))
                {
                    (text.Value as VisualText).Position.X = machine.SharedContext.DialogWindow.Position.X + (text.Value as VisualText).TextLength().X + 128;
                    (text.Value as VisualText).Position.Y = machine.SharedContext.DialogWindow.Position.Y + (txtRow * 32) + 192;
                    (text.Value as VisualText).LayerDepth = machine.SharedContext.DialogWindow.LayerDepth + (DrawingPlainOrder.PlainStep*2);
                    txtRow += 1;
                }

                if (LastDialogPortrait != null)
                {
                    if (machine.SharedContext.DialogWindow.ContainsKey(LastDialogPortrait.Id))
                    {
                        var portrait = machine.SharedContext.DialogWindow[LastDialogPortrait.Id] as ImageClient;
                        if (portrait != null)
                        {
                            
                            portrait.ApplyImageData(LastDialogPortrait);
                            portrait.Position.X = machine.SharedContext.DialogWindow.Position.X + 256;
                            portrait.Position.Y = machine.SharedContext.DialogWindow.Position.Y + (txtRow * portrait.Height) - 64;
                            portrait.LayerDepth = machine.SharedContext.DialogWindow.LayerDepth + (DrawingPlainOrder.PlainStep*2);
                            portrait.Alpha.Alpha = 1;
                        }
                    }
                    else
                    {
                        machine.SharedContext.DialogWindow.Add(LastDialogPortrait.Id, new ImageClient(LastDialogPortrait, machine.SharedContext.GameSheet.Frames));
                    }
                }
                machine.SharedContext.DialogWindow.Update(machine.SharedContext.UpdateGameTime);
            }

            if (machine.SharedContext.LastMetadataBag.CurrentDialog?.Id != LastDialogId)
            {
                if(LastDialogPortrait != null)
                {
                    machine.SharedContext.DialogWindow[LastDialogPortrait.Id].UnloadContent();
                    machine.SharedContext.DialogWindow.Remove(LastDialogPortrait.Id);
                }

                if(!string.IsNullOrWhiteSpace(LastDialogId))
                {
                    //clean head
                    machine.SharedContext.DialogWindow[LastDialogId].UnloadContent();
                    machine.SharedContext.DialogWindow.Remove(LastDialogId);
                }

                if (LastDialogTree != null)
                {
                    //clean nodes
                    var dialogIds = LastDialogTree.Select(t => t.Item1);
                    var foundDialogIds = new List<string>();
                    foreach (var vt in machine.SharedContext.DialogWindow.Where(dialogWindowObject => dialogIds.Contains(dialogWindowObject.Key)))
                    {
                        foundDialogIds.Add(vt.Key);
                        vt.Value.UnloadContent();
                    }
                    foundDialogIds.ForEach(foundDialogId => machine.SharedContext.DialogWindow.Remove(foundDialogId));

                }


                //copy dialog
                LastDialogId = machine.SharedContext.LastMetadataBag.CurrentDialog?.Id;
                LastDialogContent = machine.SharedContext.LastMetadataBag.CurrentDialog?.Content;
                LastDialogTree = machine.SharedContext.LastMetadataBag.CurrentDialog?.DialogIdContent?.ToList();
                LastDialogPortrait = machine.SharedContext.LastMetadataBag.CurrentDialog?.Portrait;

                //if (string.IsNullOrWhiteSpace(LastDialogId))
                //    machine.SharedContext.DialogWindow.Clear();

                if (LastDialogId != null)
                {
                    machine.SharedContext.DialogWindow.Add(LastDialogId, new VisualText());
                    machine.SharedContext.DialogWindow.As<VisualText>(LastDialogId).Text = LastDialogContent;
                    machine.SharedContext.DialogWindow.As<VisualText>(LastDialogId).Color = Color.Pink;
                    machine.SharedContext.DialogWindow.As<VisualText>(LastDialogId).Position.X = machine.SharedContext.DialogWindow.Position.X + machine.SharedContext.DialogWindow.Scale.X;
                    machine.SharedContext.DialogWindow.As<VisualText>(LastDialogId).Position.Y = machine.SharedContext.DialogWindow.Position.Y + machine.SharedContext.DialogWindow.Scale.Y;
                }
                if(LastDialogTree != null)
                {
                    var dialogOptionPosition = 1;
                    foreach (var newDialog in LastDialogTree)
                    {
                        machine.SharedContext.DialogWindow.Add(newDialog.Item1, new VisualText());
                        machine.SharedContext.DialogWindow.As<VisualText>(newDialog.Item1).Text = newDialog.Item2;
                        machine.SharedContext.DialogWindow.As<VisualText>(newDialog.Item1).Color = Color.Pink;
                        machine.SharedContext.DialogWindow.As<VisualText>(newDialog.Item1).Position.X = machine.SharedContext.DialogWindow.Position.X + machine.SharedContext.DialogWindow.Scale.X;
                        machine.SharedContext.DialogWindow.As<VisualText>(newDialog.Item1).Position.Y = machine.SharedContext.DialogWindow.Position.Y + machine.SharedContext.DialogWindow.Scale.Y + (dialogOptionPosition * machine.SharedContext.LastMetadataBag.CurrentDialog.FontSize);
                        dialogOptionPosition += 1;
                    }
                }
                
                
                machine.SharedContext.DialogWindow.IsVisible = LastDialogId != null;
                
            }
            
        }
    }
}
