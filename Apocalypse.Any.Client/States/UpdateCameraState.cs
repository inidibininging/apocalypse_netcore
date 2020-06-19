using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.Network;
using States.Core.Infrastructure.Services;
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
            if (machine.SharedContext.CurrentGameStateData == null)
                return;

            if (machine.SharedContext.CurrentGameStateData.Camera == null)
                return;

            if (machine.SharedContext.CurrentGameStateData.Camera.Position == null)
                return;

            if (machine.SharedContext.CurrentGameStateData.Camera.Rotation == null)
                return;

            ScreenService.Instance.DefaultScreenCamera.Position = machine.SharedContext.CurrentGameStateData.Camera.Position;            
            var playerImage = GetPlayerImage(machine);

            //ScreenService.Instance.DefaultScreenCamera.Rotation.Rotation = 0f;
            if(playerImage != null)
                //ScreenService.Instance.DefaultScreenCamera.Rotation.Rotation = playerImage.Rotation.Rotation * -0.15f;
            ScreenService.Instance.DefaultScreenCamera.Update(machine.SharedContext.UpdateGameTime);
        }
    }
}