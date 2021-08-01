using Apocalypse.Any.Client.Screens;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    ///     Starts and connects the network client with the server
    /// </summary>
    public class ConnectState : IState<string, INetworkGameScreen>
    {
        public bool ConnectAttempted { get; set; }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (ConnectAttempted)
                return;
            machine.SharedContext.Client.Start();
            machine.SharedContext.Client.Connect(machine.SharedContext.Configuration.ServerIp,
                machine.SharedContext.Configuration.ServerPort);
            ConnectAttempted = true;
        }
    }
}