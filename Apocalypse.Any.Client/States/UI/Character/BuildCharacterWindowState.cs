using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Domain.Common.Drawing.UI;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.Client.States.UI.Character
{
    public class BuildCharacterWindowState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            foreach (var kv in machine.SharedContext.GameSheet.Frames.Where(frameKV => frameKV.Key.Contains("hud_misc_edit")))
            {
                machine.SharedContext.HealthImage.SpriteSheetRectangle.Add(kv.Key, kv.Value);
                machine.SharedContext.SpeedImage.SpriteSheetRectangle.Add(kv.Key, kv.Value);
                machine.SharedContext.StrenghImage.SpriteSheetRectangle.Add(kv.Key, kv.Value);
                machine.SharedContext.DialogImage.SpriteSheetRectangle.Add(kv.Key, kv.Value);
            }
            machine.SharedContext.CharacterWindow = new ApocalypseWindow();
            machine.SharedContext.CharacterWindow.Position = new MovementBehaviour();
            machine.SharedContext.CharacterWindow.Rotation = new RotationBehaviour();
            machine.SharedContext.CharacterWindow.Color = Color.BlueViolet;
            machine.SharedContext.CharacterWindow.Scale = new Vector2(128 + 32, 64);
            machine.SharedContext.CharacterWindow.Alpha.Alpha = 1f;
            machine.SharedContext.CharacterWindow.LayerDepth = DrawingPlainOrder.UI;
            machine.SharedContext.CharacterWindow.IsVisible = true;

            machine.SharedContext.HealthImage.SelectedFrame = "hud_misc_edit_7_0";
            machine.SharedContext.SpeedImage.SelectedFrame = "hud_misc_edit_6_0";
            machine.SharedContext.StrenghImage.SelectedFrame = "hud_misc_edit_4_0";
            machine.SharedContext.DialogImage.SelectedFrame = "hud_misc_edit_7_8";

            machine.SharedContext.HealthImage.Color = Color.Red;
            machine.SharedContext.StrenghImage.Color = Color.DarkViolet;
            machine.SharedContext.SpeedImage.Color = Color.Violet;
            machine.SharedContext.DialogImage.Color = Color.BlueViolet;

            machine.SharedContext.HealthImage.LayerDepth = machine.SharedContext.CharacterWindow.LayerDepth + DrawingPlainOrder.PlainStep;
            machine.SharedContext.SpeedImage.LayerDepth = machine.SharedContext.CharacterWindow.LayerDepth + DrawingPlainOrder.PlainStep;
            machine.SharedContext.StrenghImage.LayerDepth = machine.SharedContext.CharacterWindow.LayerDepth + DrawingPlainOrder.PlainStep;
            machine.SharedContext.DialogImage.LayerDepth = machine.SharedContext.CharacterWindow.LayerDepth + DrawingPlainOrder.PlainStep;
        }
    }
}