using System;
using Apocalypse.Any.Client.Screens;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    public class BuildCameraState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            //TODO: needs decoupling screenservice dependencies
            throw new NotImplementedException();
        }
    }
}