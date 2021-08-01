using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Text;
using Apocalypse.Any.Domain.Common.Drawing.UI;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Collections.Generic;

namespace Apocalypse.Any.Client.States.UI.Info
{
    public class BuildInfoWindowState : IState<string, INetworkGameScreen>
    {
        private const string LabelName = "InfoText";
	private Dictionary<(int frame, int x, int y), Rectangle> Frames { get; set; }

	public BuildInfoWindowState(Dictionary<(int frame, int x, int y), Rectangle>  frames)
	{
	    this.Frames = frames;
	}

	public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(BuildInfoWindowState));
            machine.SharedContext.InfoWindow = new ApocalypseWindow();
            machine.SharedContext.InfoWindow.Position = new MovementBehaviour();
            machine.SharedContext.InfoWindow.Rotation = new RotationBehaviour();
            machine.SharedContext.InfoWindow.Color = Color.DarkGray;
            machine.SharedContext.InfoWindow.Scale = new Vector2(312, 486);
            machine.SharedContext.InfoWindow.Alpha.Alpha = 0.5f;
            machine.SharedContext.InfoWindow.LayerDepth = DrawingPlainOrder.UI;

            //build an info text here
            machine.SharedContext.InfoWindow.Add(LabelName, new VisualText());
            machine.SharedContext.InfoWindow.As<VisualText>(LabelName).Text = "0";
            machine.SharedContext.InfoWindow.As<VisualText>(LabelName).Color = Color.Pink;
            machine.SharedContext.InfoWindow.IsVisible = false;
        }
    }
}
