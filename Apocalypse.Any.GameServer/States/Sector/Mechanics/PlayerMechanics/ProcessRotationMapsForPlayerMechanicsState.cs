using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class ProcessRotationMapsForPlayerMechanicsState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine
               .SharedContext
               .DataLayer
               .Players
               .Where(player => !string.IsNullOrWhiteSpace(player.LoginToken))
               .ToList()
               .ForEach(player =>
               {
                   var playerGameState = machine.SharedContext.IODataLayer.GetGameStateByLoginToken(player.LoginToken);

                   playerGameState.Commands.ForEach(cmd =>
                   {
                       //pass rotation maps to server from client
                       foreach (var mappedCommand in InputMapper.DefaultRotationMap)
                       {
                           mappedCommand
                           .Translate(cmd)?
                           .ToList()
                           .ForEach(foundCmd => foundCmd.Execute(player.CurrentImage.Rotation));
                       }
                   });
               });
        }
    }
}