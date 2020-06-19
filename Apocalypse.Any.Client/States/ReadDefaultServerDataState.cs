using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model.Network;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    public class ReadDefaultServerDataState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (string.IsNullOrWhiteSpace(machine.SharedContext.Configuration.ServerIp))
                machine.SharedContext.Configuration.ServerIp = "127.0.0.1";
            if (machine.SharedContext.Configuration.ServerPort == 0)
                machine.SharedContext.Configuration.ServerPort = 8080;
            machine.SharedContext.Configuration.User = new UserData()
            {
                Username = $"foo{Randomness.Instance.From(1, 3)}",
                Password = "12345"
            };
        }
    }
}