using Apocalypse.Any.Client.Screens;
using Lidgren.Network;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    public class CheckLoginSentState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(CheckLoginSentState));
            if (machine.SharedContext.LoginSendResult != NetSendResult.Sent)
                machine.GetService.Get(ClientGameScreenBook.Login).Handle(machine);
        }
    }
}