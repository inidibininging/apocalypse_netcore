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
        private string LastDialogId { get; set; }
        private string LastDialogContent { get; set; }
        private List<Tuple<string, string>> LastDialogTree { get; set; }
        private ImageData LastDialogPortrait { get; set; }
        private Dictionary<string,int> LastDialogPositions { get; set; } = new Dictionary<string, int>();

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.DialogWindow == null)
                return;

            if (machine.SharedContext.LastMetadataBag == null)
            {
                machine.SharedContext.DialogWindow.IsVisible = false;
                return;
            }
            if (machine.SharedContext.DialogWindow.IsVisible)
            {
                //move the dialog in front of players ship
                var playerImage = machine.SharedContext.Images.FirstOrDefault(img => img.ServerData.Id == machine.SharedContext.PlayerImageId);
                machine.SharedContext.DialogWindow.Position.X = playerImage.Position.X - 256;
                machine.SharedContext.DialogWindow.Position.Y = playerImage.Position.Y - 486;
                machine.SharedContext.DialogWindow.LayerDepth = DrawingPlainOrder.UI - 0.1f;
                machine.SharedContext.DialogWindow.Alpha.Alpha = 0.5f;

                //move title according to the window position
                if (!string.IsNullOrWhiteSpace(LastDialogId))
                {
                    (machine.SharedContext.DialogWindow[LastDialogId] as VisualText).Position.X = machine.SharedContext.DialogWindow.Position.X + (machine.SharedContext.DialogWindow[LastDialogId] as VisualText).TextLength().X + 128;
                    (machine.SharedContext.DialogWindow[LastDialogId] as VisualText).Position.Y = machine.SharedContext.DialogWindow.Position.Y + (machine.SharedContext.DialogWindow[LastDialogId] as VisualText).TextLength().Y + 128;
                    (machine.SharedContext.DialogWindow[LastDialogId] as VisualText).LayerDepth = machine.SharedContext.DialogWindow.LayerDepth + (DrawingPlainOrder.PlainStep * 2);
                }

                //move answers according to the window position
                foreach (var text in machine.SharedContext.DialogWindow.Where(txts => txts.Value.GetType() == typeof(VisualText)))
                {
                    //skip the title
                    if (!LastDialogPositions.ContainsKey(text.Key))
                        continue;
                    (text.Value as VisualText).Position.X = machine.SharedContext.DialogWindow.Position.X + (text.Value as VisualText).TextLength().X + 128;
                    (text.Value as VisualText).Position.Y = machine.SharedContext.DialogWindow.Position.Y + (LastDialogPositions[text.Key] * 32) + 192;
                    (text.Value as VisualText).LayerDepth = machine.SharedContext.DialogWindow.LayerDepth + (DrawingPlainOrder.PlainStep*2);
                }

                //move the portrait according to the window position
                if (LastDialogPortrait != null)
                {
                    if (machine.SharedContext.DialogWindow.ContainsKey(LastDialogPortrait.Id))
                    {
                        var portrait = machine.SharedContext.DialogWindow[LastDialogPortrait.Id] as ImageClient;
                        if (portrait != null)
                        {
                            //portrait.ApplyImageData(LastDialogPortrait);
                            portrait.Position.X = machine.SharedContext.DialogWindow.Position.X + 320;
                            portrait.Position.Y = machine.SharedContext.DialogWindow.Position.Y - (portrait.Height + 64);
                            portrait.LayerDepth = machine.SharedContext.DialogWindow.LayerDepth + (DrawingPlainOrder.PlainStep*2);
                            portrait.Alpha.Alpha = 1;
                        }
                    }
                    else
                    {
                        machine.SharedContext.DialogWindow.Add(LastDialogPortrait.Id, new ImageClient(LastDialogPortrait, machine.SharedContext.GameSheet.Frames));
                    }
                }

                //build corners
                var upperCornerLeft = (machine.SharedContext.DialogWindow["upperCornerLeft"] as SpriteSheet);
                var upperCornerRight = (machine.SharedContext.DialogWindow["upperCornerRight"] as SpriteSheet);
                var lowerCornerLeft = (machine.SharedContext.DialogWindow[ "lowerCornerLeft"] as SpriteSheet);
                var lowerCornerRight = (machine.SharedContext.DialogWindow["lowerCornerRight"] as SpriteSheet);

                upperCornerLeft.Position.X = machine.SharedContext.DialogWindow.Position.X + 32;
                upperCornerLeft.Position.Y = machine.SharedContext.DialogWindow.Position.Y + 32;
                upperCornerRight.Position.X = machine.SharedContext.DialogWindow.Position.X + 640 - 32;
                upperCornerRight.Position.Y = machine.SharedContext.DialogWindow.Position.Y + 32;
                lowerCornerLeft.Position.X = machine.SharedContext.DialogWindow.Position.X + 32;
                lowerCornerLeft.Position.Y = machine.SharedContext.DialogWindow.Position.Y + 448 - 32;
                lowerCornerRight.Position.X = machine.SharedContext.DialogWindow.Position.X + 640 - 32;
                lowerCornerRight.Position.Y = machine.SharedContext.DialogWindow.Position.Y + 448 - 32;

                //build  walls

                //build upper wall of dialog
                for (int dialogWall = 1; dialogWall <= (machine.SharedContext.DialogWindow.Scale.X / 64) - 2; dialogWall++)
                {
                    (machine.SharedContext.DialogWindow[$"upperWall_{dialogWall}"] as SpriteSheet).Alpha.Alpha = 1f;
                    (machine.SharedContext.DialogWindow[$"upperWall_{dialogWall}"] as SpriteSheet).Position.X = machine.SharedContext.DialogWindow.Position.X + (64 * dialogWall) + 32;
                    (machine.SharedContext.DialogWindow[$"upperWall_{dialogWall}"] as SpriteSheet).Position.Y = machine.SharedContext.DialogWindow.Position.Y + 32;
                }

                //build lower wall of dialog
                for (int dialogWall = 1; dialogWall <= (machine.SharedContext.DialogWindow.Scale.X / 64) - 2; dialogWall++)
                {
                    (machine.SharedContext.DialogWindow[$"lowerWall_{dialogWall}"] as SpriteSheet).Alpha.Alpha = 1f;
                    (machine.SharedContext.DialogWindow[$"lowerWall_{dialogWall}"] as SpriteSheet).Position.X = machine.SharedContext.DialogWindow.Position.X + (64 * dialogWall) + 32;
                    (machine.SharedContext.DialogWindow[$"lowerWall_{dialogWall}"] as SpriteSheet).Position.Y = machine.SharedContext.DialogWindow.Position.Y + machine.SharedContext.DialogWindow.Scale.Y - 32;
                    (machine.SharedContext.DialogWindow[$"lowerWall_{dialogWall}"] as SpriteSheet).Rotation.Rotation = 62.5f;
                }

                //build left wall of dialog
                for (int dialogWall = 1; dialogWall <= (machine.SharedContext.DialogWindow.Scale.X / 64) - 5; dialogWall++)
                {
                    (machine.SharedContext.DialogWindow[$"leftWall_{dialogWall}"] as SpriteSheet).Alpha.Alpha = 1f;
                    (machine.SharedContext.DialogWindow[$"leftWall_{dialogWall}"] as SpriteSheet).Position.X = machine.SharedContext.DialogWindow.Position.X + 32;
                    (machine.SharedContext.DialogWindow[$"leftWall_{dialogWall}"] as SpriteSheet).Position.Y = machine.SharedContext.DialogWindow.Position.Y + (64 * dialogWall) + 32;
                    (machine.SharedContext.DialogWindow[$"leftWall_{dialogWall}"] as SpriteSheet).Rotation.Rotation = 31.5f;
                }

                //build right wall of dialog
                for (int dialogWall = 1; dialogWall <= (machine.SharedContext.DialogWindow.Scale.X / 64) - 5; dialogWall++)
                {
                    (machine.SharedContext.DialogWindow[$"rightWall_{dialogWall}"] as SpriteSheet).Alpha.Alpha = 1f;
                    (machine.SharedContext.DialogWindow[$"rightWall_{dialogWall}"] as SpriteSheet).Position.X = machine.SharedContext.DialogWindow.Position.X + machine.SharedContext.DialogWindow.Scale.X - 32;
                    (machine.SharedContext.DialogWindow[$"rightWall_{dialogWall}"] as SpriteSheet).Position.Y = machine.SharedContext.DialogWindow.Position.Y + (64 * dialogWall) + 32;
                    (machine.SharedContext.DialogWindow[$"rightWall_{dialogWall}"] as SpriteSheet).Rotation.Rotation = 31.5f;
                }

                //(machine.SharedContext.DialogWindow[$"lowerWall_{dialogWall}"] as SpriteSheet).Rotation.Rotation = 30;

                machine.SharedContext.DialogWindow.Update(machine.SharedContext.UpdateGameTime);
            }

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
                    foreach (var vt in machine.SharedContext.DialogWindow.Where(dialogWindowObject => dialogIds.Contains(dialogWindowObject.Key)))
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
                    machine.SharedContext.DialogWindow.As<VisualText>(LastDialogId).Position.X = machine.SharedContext.DialogWindow.Position.X + machine.SharedContext.DialogWindow.Scale.X;
                    machine.SharedContext.DialogWindow.As<VisualText>(LastDialogId).Position.Y = machine.SharedContext.DialogWindow.Position.Y + machine.SharedContext.DialogWindow.Scale.Y;
                }

                //create the dialog tree
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
                        LastDialogPositions.Add(newDialog.Item1, dialogOptionPosition);
                    }
                }
                
                
                machine.SharedContext.DialogWindow.IsVisible = LastDialogId != null;
                
            }
            
        }
    }
}
