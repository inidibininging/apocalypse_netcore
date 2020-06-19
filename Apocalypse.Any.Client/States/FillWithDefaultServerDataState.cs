using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Core.Utilities;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// Fills empty connection variables (ip, port, peer, username, pass) with default values
    /// </summary>
    public class FillWithDefaultServerDataState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (string.IsNullOrWhiteSpace(machine.SharedContext.Configuration.ServerIp))
                machine.SharedContext.Configuration.ServerIp = "127.0.0.1";

            if (machine.SharedContext.Configuration.ServerPort <= 0)
                machine.SharedContext.Configuration.ServerPort = 8080;

            if (string.IsNullOrWhiteSpace(machine.SharedContext.Configuration.User.Username))
                machine.SharedContext.Configuration.User.Username = $"foo{Randomness.Instance.From(0, 3).ToString()}";

            if (string.IsNullOrWhiteSpace(machine.SharedContext.Configuration.User.Password))
                machine.SharedContext.Configuration.User.Password = "12345";
        }
    }
}