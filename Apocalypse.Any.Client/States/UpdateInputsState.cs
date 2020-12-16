using Apocalypse.Any.Client.Screens;

using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Updates the InputService
    /// </summary>
    public class UpdateInputsState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.InputService.Update(machine.SharedContext.UpdateGameTime);

            //handle zoom in zoom out of camera
            
        }
    }
}