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
            machine.SharedContext.DialogWindow.Scale = new Vector2(640, 480);
            machine.SharedContext.DialogWindow.Alpha.Alpha = 0.5f;
            machine.SharedContext.DialogWindow.LayerDepth = DrawingPlainOrder.UI;

            //TODO: Draw the background
            
        }
    }
}
