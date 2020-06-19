using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class ProcessUseInventoryForPlayerState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine
                 .SharedContext
                 .DataLayer
                 .Players
                 .ToList()
                 .ForEach(player =>
                 {
                     machine
                    .SharedContext
                    .IODataLayer
                    .GetGameStateByLoginToken(player.LoginToken)?
                    .Commands.ForEach(cmd =>
                    {
                        if (cmd.Contains(DefaultKeys.Use))
                        {
                            var useId = cmd.Replace(DefaultKeys.Use, "");
                            if (string.IsNullOrWhiteSpace(useId))
                                return;
                        }
                    });
                 });
        }
    }
}