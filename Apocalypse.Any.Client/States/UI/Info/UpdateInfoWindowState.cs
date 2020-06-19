using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Text;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Network;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.Client.States.UI.Info
{
    public class UpdateInfoWindowState : IState<string, INetworkGameScreen>
    {
        private const string LabelName = "InfoText";

        private ImageClient GetPlayerImage(IStateMachine<string, INetworkGameScreen> machine)
        {
            return machine.SharedContext.Images.Where(oldImg => oldImg.ServerData.Id == machine.SharedContext.PlayerImageId).FirstOrDefault();
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {            
            machine.SharedContext.Messages.Add(nameof(UpdateInfoWindowState));

            var player = GetPlayerImage(machine);
            if (player == null)
                return;
            if (player.Position == null)
                return;

            if (machine.SharedContext.InputService.InputNow.ToList().Contains(Apocalypse.Any.Core.Input.Translator.DefaultKeys.CloseInfo))
            {
                machine.SharedContext.InfoWindow.IsVisible = false;
                return;
            }

            if (machine.SharedContext.InputService.InputNow.ToList().Contains(Apocalypse.Any.Core.Input.Translator.DefaultKeys.OpenInfo))
            {
                machine.SharedContext.InfoWindow.IsVisible = true;
            }

            var offsetLeftX = 128;
            machine.SharedContext.InfoWindow.Position.X = player.Position.X - (288) - offsetLeftX;
            machine.SharedContext.InfoWindow.Position.Y = player.Position.Y - 386;

            var infoText = machine.SharedContext.InfoWindow.As<VisualText>(LabelName);
            infoText.Text = string.Join(System.Environment.NewLine, machine.SharedContext.Messages);
            infoText.Position.X = player.Position.X - 16 - offsetLeftX;//$"Experience:{machine.SharedContext.CurrentSheetSnapshot.Experience}";//ddddd
            infoText.Position.Y = player.Position.Y + 96;
            infoText.Scale = new Vector2(0.75f);
            infoText.LayerDepth = machine.SharedContext.InfoWindow.LayerDepth + (DrawingPlainOrder.PlainStep);

            machine.SharedContext.InfoWindow.Update(machine.SharedContext.UpdateGameTime);
        }
    }
}