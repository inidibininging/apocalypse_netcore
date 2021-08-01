using Apocalypse.Any.Client.Screens;
using States.Core.Common;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    ///     Main client game context. Holds a shared context (the network game screen)
    ///     Its kind of a replacer of the screen service
    /// </summary>
    public class ClientGameContext : StateMachine<string, INetworkGameScreen>
    {
        public ClientGameContext(
            IStateGetService<string, INetworkGameScreen> getService,
            IStateSetService<string, INetworkGameScreen> setService,
            IStateNewService<string, INetworkGameScreen> newService)
            : base(getService, setService, newService)
        {
        }
    }
}