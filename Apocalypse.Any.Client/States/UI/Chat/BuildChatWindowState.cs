using System;
using Apocalypse.Any.Client.Screens;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States.UI.Chat
{
    public class BuildChatWindowState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            throw new NotImplementedException();
        }
    }
}