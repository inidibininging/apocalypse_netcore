using System.Collections.Generic;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Text;
using Apocalypse.Any.Domain.Common.Drawing.UI;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Linq;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Client.States.UI.Character
{
    public class BuildCharacterWindowState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            foreach (var kv in machine.SharedContext.GameSheet.Frames.Where(frameKV => frameKV.Key.frame == ImagePaths.HUDFrame))
            {
                machine.SharedContext.HealthImage.SpriteSheetRectangle.Add(kv.Key, kv.Value);
                machine.SharedContext.SpeedImage.SpriteSheetRectangle.Add(kv.Key, kv.Value);
                machine.SharedContext.StrenghImage.SpriteSheetRectangle.Add(kv.Key, kv.Value);
                machine.SharedContext.DialogImage.SpriteSheetRectangle.Add(kv.Key, kv.Value);
            }

            machine.SharedContext.MoneyCount = new VisualText();

            machine.SharedContext.CharacterWindow = new ApocalypseWindow
            {
                Position = new MovementBehaviour(),
                Rotation = new RotationBehaviour(),
                Color = Color.BlueViolet,
                Scale = new Vector2(128 + 32, 64),
                Alpha = {Alpha = 1f},
                LayerDepth = DrawingPlainOrder.UI,
                IsVisible = true
            };

            machine.SharedContext.HealthImage.SelectedFrame = (ImagePaths.HUDFrame, 7, 0);
            machine.SharedContext.SpeedImage.SelectedFrame = (ImagePaths.HUDFrame, 6, 0);
            machine.SharedContext.StrenghImage.SelectedFrame = (ImagePaths.HUDFrame, 4, 0);
            machine.SharedContext.DialogImage.SelectedFrame = (ImagePaths.HUDFrame, 7, 8);

            machine.SharedContext.HealthImage.Color = Color.Red;
            machine.SharedContext.StrenghImage.Color = Color.DarkViolet;
            machine.SharedContext.SpeedImage.Color = Color.Violet;
            machine.SharedContext.DialogImage.Color = Color.BlueViolet;
            machine.SharedContext.MoneyCount.Color = Color.Yellow;

            machine.SharedContext.HealthImage.LayerDepth = machine.SharedContext.CharacterWindow.LayerDepth + DrawingPlainOrder.PlainStep;
            machine.SharedContext.SpeedImage.LayerDepth = machine.SharedContext.CharacterWindow.LayerDepth + DrawingPlainOrder.PlainStep;
            machine.SharedContext.StrenghImage.LayerDepth = machine.SharedContext.CharacterWindow.LayerDepth + DrawingPlainOrder.PlainStep;
            machine.SharedContext.DialogImage.LayerDepth = machine.SharedContext.CharacterWindow.LayerDepth + DrawingPlainOrder.PlainStep;
            machine.SharedContext.MoneyCount.LayerDepth = machine.SharedContext.CharacterWindow.LayerDepth + DrawingPlainOrder.PlainStep;
            
            //TODO: can cause maybe a bug.
            machine.SharedContext.Add("testButton",new ApocalypseButton<string>(machine.SharedContext.GameSheet.Frames));
        }
    }
}