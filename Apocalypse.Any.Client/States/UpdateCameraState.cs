using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.Network;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;
using Apocalypse.Any.Core.Input.Translator;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Updates the camera state with data from server
    /// </summary>
    public class UpdateCameraState : IState<string, INetworkGameScreen>
    {
        private ImageClient GetPlayerImage(IStateMachine<string, INetworkGameScreen> machine)
        {
            return machine.SharedContext.Images.FirstOrDefault(oldImg => oldImg.ServerData.Id == machine.SharedContext.PlayerImageId);
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.CurrentGameStateData?.Camera?.Position == null)
                return;

            if (machine.SharedContext.CurrentGameStateData.Camera?.Rotation == null)
                return;

            
            foreach (var input in machine.SharedContext.InputService.InputBefore)
            {
                if(input.Contains(DefaultKeys.ZoomIn))
                    ScreenService.Instance.DefaultScreenCamera.ZoomIn();
                if(input.Contains(DefaultKeys.ZoomOut))
                    ScreenService.Instance.DefaultScreenCamera.ZoomOut();
            }
            var playerImage = GetPlayerImage(machine);
            if (playerImage != null)
            {
                ScreenService.Instance.DefaultScreenCamera.Position.X = MathHelper.Lerp(ScreenService.Instance.DefaultScreenCamera.Position.X, machine.SharedContext.CursorImage.Position.X, 0.05f);
                ScreenService.Instance.DefaultScreenCamera.Position.Y = MathHelper.Lerp(ScreenService.Instance.DefaultScreenCamera.Position.Y, machine.SharedContext.CursorImage.Position.Y, 0.05f);
                ScreenService.Instance.DefaultScreenCamera.Update(machine.SharedContext.UpdateGameTime);
            }

        }
    }
}