using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Linq;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Client.States.UI
{
    public class BuildCursorState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.CursorImage is SpriteSheet sheet)
            {
                foreach (var kv in machine.SharedContext.GameSheet.Frames.Where(frameKV => frameKV.Key.frame == ImagePaths.HUDFrame))
                {
                    sheet.SpriteSheetRectangle.Add(kv.Key, kv.Value);
                }
                sheet.SelectedFrame = (ImagePaths.HUDFrame, 4, 6);    
            }
            
            machine.SharedContext.CursorImage.Color = Color.Violet;
            machine.SharedContext.CursorImage.Scale = new Vector2(0.25f);
            machine.SharedContext.CursorImage.LayerDepth = DrawingPlainOrder.UI + DrawingPlainOrder.PlainStep;
        }
    }
}