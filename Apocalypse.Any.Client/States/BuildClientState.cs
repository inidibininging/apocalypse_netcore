using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Domain.Client.Model;
using Lidgren.Network;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    public class BuildClientState : IState<string, INetworkGameScreen>
    {
        public GameClientConfiguration ClientConfiguration { get; }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Client = new NetClient(
                new NetPeerConfiguration(machine.SharedContext.Configuration.ServerPeerName)
                {
                    EnableUPnP = true,
                    AutoFlushSendQueue = true
                });
        }
    }
}