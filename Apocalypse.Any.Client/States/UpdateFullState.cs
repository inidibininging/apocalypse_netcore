using Apocalypse.Any.Client.Screens;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    public class UpdateFullState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(UpdateFullState));
            machine.GetService.Get(ClientGameScreenBook.UpdateInput).Handle(machine);
            machine.GetService.Get(ClientGameScreenBook.UpdateCursor).Handle(machine);

            machine.GetService.Get(ClientGameScreenBook.FetchData).Handle(machine);
            machine.GetService.Get(ClientGameScreenBook.UpdateImages).Handle(machine);
            machine.GetService.Get(ClientGameScreenBook.UpdateMetadataState).Handle(machine);
            machine.GetService.Get(ClientGameScreenBook.UpdateScreen).Handle(machine);
            machine.GetService.Get(ClientGameScreenBook.SendGameStateUpdateData).Handle(machine);
            machine.GetService.Get(ClientGameScreenBook.UpdateUI).Handle(machine);
        }
    }
}