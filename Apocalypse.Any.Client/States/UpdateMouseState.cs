using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.Network;
using Microsoft.Xna.Framework.Input;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Updates the mouse state and the cursor image(s)
    /// </summary>
    public class UpdateCursorState : IState<string, INetworkGameScreen>
    {
        private ImageClient GetPlayerImage(IStateMachine<string, INetworkGameScreen> machine)
        {
            return machine.SharedContext.Images.Where(oldImg => oldImg.ServerData.Id == machine.SharedContext.PlayerImageId).FirstOrDefault();
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(UpdateCursorState));

            var playerImage = GetPlayerImage(machine);

            if (playerImage == null)
                return;

            var mouseState = Mouse.GetState();
            machine.SharedContext.Messages.Add($"{mouseState.X},{mouseState.Y}");
            
            machine.SharedContext.CursorImage.Position.X = (float)mouseState.X + playerImage.Position.X - (ScreenService.Instance.Resolution.X/2);
            machine.SharedContext.CursorImage.Position.Y = (float)mouseState.Y + playerImage.Position.Y - (ScreenService.Instance.Resolution.Y/2);
            var CursorImage = machine.SharedContext.CursorImage.Position.ToVector2();

            // var lerp = Vector2.Lerp(CursorImage,playerImage.Position,0.2f);
            // machine.SharedContext.LerpCursorImage.Position.X = lerp.X;
            // machine.SharedContext.LerpCursorImage.Position.Y = lerp.Y;


            //facepoint mechanics
            var targetVector = machine.SharedContext.CursorImage.Position.ToVector2() - playerImage.Position.ToVector2();
            var faceRotation = MathF.Atan2(targetVector.Y, targetVector.X) * (180f / MathF.PI);


            machine.SharedContext.CursorImage.Rotation.Rotation = faceRotation;//playerImage.Rotation.Rotation.ToOppositeAngle();//faceRotation;// < 0 ? 180 + MathF.Abs(faceRotation) : faceRotation;
            // machine.SharedContext.LerpCursorImage.Rotation.Rotation = faceRotation;

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                //mouse right from player
                if (machine.SharedContext.CursorImage.Position.X > playerImage.Position.X)
                    machine.SharedContext.InputService.InputNow.Append(DefaultKeys.Right);

                //mouse left from player
                if (machine.SharedContext.CursorImage.Position.X < playerImage.Position.X)
                    machine.SharedContext.InputService.InputNow.Append(DefaultKeys.Left);
            }

            machine.SharedContext.Messages.Add($"{playerImage.Rotation.Rotation}");
        }
    }
}