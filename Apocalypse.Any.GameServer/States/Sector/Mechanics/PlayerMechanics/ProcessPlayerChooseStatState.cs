using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class ProcessPlayerChooseStatState : IState<string, IGameSectorLayerService>
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
                    if (cmd == DefaultKeys.ChooseHealth)
                        player.ChosenStat = "Health";

                    if (cmd == DefaultKeys.ChooseSpeed)
                        player.ChosenStat = "Speed";

                    if (cmd == DefaultKeys.ChooseStrength)
                        player.ChosenStat = "Strength";

                    if (cmd == DefaultKeys.ChooseArmor)
                        player.ChosenStat = "Armor";
                });
            });
        }
    }
}