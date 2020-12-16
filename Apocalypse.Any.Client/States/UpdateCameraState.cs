using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.Network;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Updates the camera state with data from server
    /// </summary>
    public class UpdateCameraState : IState<string, INetworkGameScreen>
    {
        private ImageClient GetPlayerImage(IStateMachine<string, INetworkGameScreen> machine)
        {
            return machine.SharedContext.Images.Where(oldImg => oldImg.ServerData.Id == machine.SharedContext.PlayerImageId).FirstOrDefault();
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.CurrentGameStateData?.Camera?.Position == null)
                return;

            if (machine.SharedContext.CurrentGameStateData.Camera?.Rotation == null)
                return;

            var playerImage = GetPlayerImage(machine);
            if (playerImage != null)
            {

                ScreenService.Instance.DefaultScreenCamera.Position.X = MathHelper.Lerp(ScreenService.Instance.DefaultScreenCamera.Position.X, machine.SharedContext.CursorImage.Position.X, 0.05f);
                ScreenService.Instance.DefaultScreenCamera.Position.Y = MathHelper.Lerp(ScreenService.Instance.DefaultScreenCamera.Position.Y, machine.SharedContext.CursorImage.Position.Y, 0.05f);
                //if (ScreenService.Instance.DefaultScreenCamera.ZoomDifference <= 0.0422507524)
                //{
                //    var distance = Vector2.Distance(playerImage.Position, ScreenService.Instance.DefaultScreenCamera.Position);
                //    if (distance >= 0.2)
                //        ScreenService.Instance.DefaultScreenCamera.Zoom = 1f + (1.5f / distance);
                //}
                ScreenService.Instance.DefaultScreenCamera.Update(machine.SharedContext.UpdateGameTime);
            }

            //ScreenService.Instance.DefaultScreenCamera.Rotation.Rotation = 0f;
            
                //ScreenService.Instance.DefaultScreenCamera.Rotation.Rotation = playerImage.Rotation.Rotation * -0.15f;
        }
    }
}