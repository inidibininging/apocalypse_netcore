using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Text;
using Apocalypse.Any.Domain.Common.Drawing.UI;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Client.States.UI.Dialog
{
    public class BuildDialogWindowState : IState<string, INetworkGameScreen>
    {
        private const string LabelName = "DialogText";

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(BuildDialogWindowState));
            machine.SharedContext.DialogWindow = new ApocalypseWindow();
            machine.SharedContext.DialogWindow.Position = new MovementBehaviour();
            machine.SharedContext.DialogWindow.Rotation = new RotationBehaviour();
            machine.SharedContext.DialogWindow.Color = Color.BlueViolet;
            machine.SharedContext.DialogWindow.Scale = new Vector2(640, 448 + 64);
            machine.SharedContext.DialogWindow.Alpha.Alpha = 0.5f;
            machine.SharedContext.DialogWindow.LayerDepth = DrawingPlainOrder.UI;

            //TODO: Draw the background
            var upperCornerLeft = new SpriteSheet(machine.SharedContext.GameSheet.Frames)
            {
                Path = ImagePaths.dialogue,
                SelectedFrame = (ImagePaths.DialogueFrame, 0 ,0),
                LayerDepth = DrawingPlainOrder.UI,
                ForceDraw = true,
                Position = new MovementBehaviour()
                {
                    X = machine.SharedContext.DialogWindow.Position.X,
                    Y = machine.SharedContext.DialogWindow.Position.Y
                },
                Scale = new Vector2(2, 2),
                Color = Color.DarkViolet
            };
            var upperCornerRight = new SpriteSheet(machine.SharedContext.GameSheet.Frames)
            {
                Path = ImagePaths.dialogue,
                SelectedFrame = (ImagePaths.DialogueFrame, 0 ,1),
                LayerDepth = DrawingPlainOrder.UI,
                ForceDraw = true,
                Position = new MovementBehaviour()
                {
                    X = machine.SharedContext.DialogWindow.Position.X + 640,
                    Y = machine.SharedContext.DialogWindow.Position.Y
                },
                Scale = new Vector2(2, 2),
                Color = Color.DarkViolet
            };

            var lowerCornerLeft = new SpriteSheet(machine.SharedContext.GameSheet.Frames)
            {
                Path = ImagePaths.dialogue,
                SelectedFrame = (ImagePaths.DialogueFrame, 0 ,2),
                LayerDepth = DrawingPlainOrder.UI,
                ForceDraw = true,
                Position = new MovementBehaviour()
                {
                    X = machine.SharedContext.DialogWindow.Position.X ,
                    Y = machine.SharedContext.DialogWindow.Position.Y + 448
                },
                Scale = new Vector2(2, 2),
                Color = Color.DarkViolet
            };
            var lowerCornerRight = new SpriteSheet(machine.SharedContext.GameSheet.Frames)
            {
                Path = ImagePaths.dialogue,
                SelectedFrame = (ImagePaths.DialogueFrame, 0 ,3),
                LayerDepth = DrawingPlainOrder.UI,
                ForceDraw = true,
                Position = new MovementBehaviour()
                {
                    X = machine.SharedContext.DialogWindow.Position.X + 640,
                    Y = machine.SharedContext.DialogWindow.Position.Y + 448
                },
                Scale = new Vector2(2, 2),
                Color = Color.DarkViolet
            };

            //build upper wall of dialog
            for (int dialogWall = 1; dialogWall <= (machine.SharedContext.DialogWindow.Scale.X / 64) - 2; dialogWall++)
            {
                var upperWall = new SpriteSheet(machine.SharedContext.GameSheet.Frames)
                {
                    Path = ImagePaths.dialogue,
                    SelectedFrame = (ImagePaths.DialogueFrame, 0 ,4),
                    LayerDepth = DrawingPlainOrder.UI + DrawingPlainOrder.PlainStep,
                    ForceDraw = true,
                    Position = new MovementBehaviour()
                    {
                        X = machine.SharedContext.DialogWindow.Position.X + (64 * dialogWall),
                        Y = machine.SharedContext.DialogWindow.Position.Y
                    },
                    Scale = new Vector2(2, 2),                    
                    Color = Color.DarkViolet
                };
                machine.SharedContext.DialogWindow.Add($"{nameof(upperWall)}_{dialogWall}", upperWall);
            }

            //build lower wall of dialog
            for (int dialogWall = 1; dialogWall <= (machine.SharedContext.DialogWindow.Scale.X / 64) - 2; dialogWall++)
            {
                var lowerWall = new SpriteSheet(machine.SharedContext.GameSheet.Frames)
                {
                    Path = ImagePaths.dialogue,
                    SelectedFrame = (ImagePaths.DialogueFrame, 0 ,4),
                    LayerDepth = DrawingPlainOrder.UI + DrawingPlainOrder.PlainStep,
                    ForceDraw = true,
                    Position = new MovementBehaviour()
                    {
                        X = machine.SharedContext.DialogWindow.Position.X + (64 * dialogWall),
                        Y = machine.SharedContext.DialogWindow.Position.Y + machine.SharedContext.DialogWindow.Scale.Y
                    },
                    Rotation = new RotationBehaviour() { Rotation = 0 },
                    Scale = new Vector2(2, 2),
                    Color = Color.DarkViolet
                };
                machine.SharedContext.DialogWindow.Add($"{nameof(lowerWall)}_{dialogWall}", lowerWall);
            }


            //build left wall of dialog
            for (int dialogWall = 1; dialogWall <= (machine.SharedContext.DialogWindow.Scale.X / 64) - 2; dialogWall++)
            {
                var leftWall = new SpriteSheet(machine.SharedContext.GameSheet.Frames)
                {
                    Path = ImagePaths.dialogue,
                    SelectedFrame = (ImagePaths.DialogueFrame, 0 ,4),
                    LayerDepth = DrawingPlainOrder.UI + DrawingPlainOrder.PlainStep,
                    ForceDraw = true,
                    Position = new MovementBehaviour()
                    {
                        X = machine.SharedContext.DialogWindow.Position.X,// + machine.SharedContext.DialogWindow.Scale.Y,
                        Y = machine.SharedContext.DialogWindow.Position.Y + (64 * dialogWall),
                    },
                    Rotation = new RotationBehaviour() { Rotation = 30 },
                    Scale = new Vector2(2, 2),
                    Color = Color.DarkViolet
                };
                machine.SharedContext.DialogWindow.Add($"{nameof(leftWall)}_{dialogWall}", leftWall);
            }

            //build right wall of dialog
            for (int dialogWall = 1; dialogWall <= (machine.SharedContext.DialogWindow.Scale.X / 64) - 2; dialogWall++)
            {
                var rightWall = new SpriteSheet(machine.SharedContext.GameSheet.Frames)
                {
                    Path = ImagePaths.dialogue,
                    SelectedFrame = (ImagePaths.DialogueFrame, 0, 4),
                    LayerDepth = DrawingPlainOrder.UI + DrawingPlainOrder.PlainStep,
                    ForceDraw = true,
                    Position = new MovementBehaviour()
                    {
                        X = machine.SharedContext.DialogWindow.Position.X + machine.SharedContext.DialogWindow.Scale.X - 32,
                        Y = machine.SharedContext.DialogWindow.Position.Y + (64 * dialogWall),
                    },
                    Rotation = new RotationBehaviour() { Rotation = 30 },
                    Scale = new Vector2(2, 2),
                    Color = Color.DarkViolet
                };
                machine.SharedContext.DialogWindow.Add($"{nameof(rightWall)}_{dialogWall}", rightWall);
            }

            machine.SharedContext.DialogWindow.Add(nameof(upperCornerLeft),  upperCornerLeft);
            machine.SharedContext.DialogWindow.Add(nameof(upperCornerRight), upperCornerRight);
            machine.SharedContext.DialogWindow.Add(nameof(lowerCornerLeft),  lowerCornerLeft);
            machine.SharedContext.DialogWindow.Add(nameof(lowerCornerRight), lowerCornerRight);

        }
    }
}
