using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    /// <summary>
    /// This state is only for serverside client that get server authorative commands
    /// </summary>
    public class ProcessKeyDownState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            //TODO:
            //1. get key down and remember
            //2. if release pressed kill it and stop passing releases 
            throw new System.NotImplementedException();
        }
    }
}