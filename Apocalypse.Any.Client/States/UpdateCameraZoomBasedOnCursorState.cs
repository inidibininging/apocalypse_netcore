using System;
using System.Linq;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    public class UpdateCameraZoomBasedOnCursorState : IState<string, INetworkGameScreen>
    {
        private ImageClient GetPlayerImage(IStateMachine<string, INetworkGameScreen> machine)
        {
            return machine.SharedContext.Images.FirstOrDefault(oldImg => oldImg.ServerData.Id == machine.SharedContext.PlayerImageId);
        }

        private FloatDeltaService ZoomDerivative1 { get; set; } = new FloatDeltaService();
        
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.CursorImage == null)
                return;
            var playerImage = GetPlayerImage(machine);
            if (playerImage == null)
                return;

            var distance = Vector2.Distance(playerImage.Position - playerImage.Scale,
                ScreenService.Instance.DefaultScreenCamera.Position.ToVector2());

            // why this magic?
            const float unknowParameter = 1500f;
            distance /= unknowParameter;
            distance = MathF.Round(distance,1);
            ZoomDerivative1.Update(distance);

            var zoomDirection = (int)MathF.Round(ZoomDerivative1.Delta * 10);
            const int zoomMaxDeltaThresholdValue = 1;
            
            if (zoomDirection > zoomMaxDeltaThresholdValue || zoomDirection < zoomMaxDeltaThresholdValue * -1)
                return;
            
            const float zoomFactor = 0.5f;

            if (ScreenService.Instance.DefaultScreenCamera.Position.X > playerImage.Position.X)
            {
                if (zoomDirection > 0)
                    ScreenService.Instance.DefaultScreenCamera.ZoomOut(zoomFactor);
            
                if (zoomDirection < 0)
                    ScreenService.Instance.DefaultScreenCamera.ZoomIn(zoomFactor);
            }
                        
            if (ScreenService.Instance.DefaultScreenCamera.Position.X < playerImage.Position.X)
            {
                if (zoomDirection < 0)
                    ScreenService.Instance.DefaultScreenCamera.ZoomIn(zoomFactor);
            
                if (zoomDirection > 0)
                    ScreenService.Instance.DefaultScreenCamera.ZoomOut(zoomFactor);
            }
            
            
        }
    }
}