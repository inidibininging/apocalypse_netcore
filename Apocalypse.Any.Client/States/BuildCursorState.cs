using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.Client.States.UI
{
    public class BuildCursorState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            foreach (var kv in machine.SharedContext.GameSheet.Frames.Where(frameKV => frameKV.Key.Contains("hud_misc_edit")))
            {
                machine.SharedContext.CursorImage.SpriteSheetRectangle.Add(kv.Key, kv.Value);
            }
            machine.SharedContext.CursorImage.SelectedFrame = "hud_misc_edit_4_6";
            machine.SharedContext.CursorImage.Color = Color.Violet;
            machine.SharedContext.CursorImage.Scale = new Vector2(0.25f);
            machine.SharedContext.CursorImage.LayerDepth = DrawingPlainOrder.UI + DrawingPlainOrder.PlainStep;
        }
    }
}