using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics
{
    public class ProcessReleaseStatState : IState<string, IGameSectorLayerService>
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
                        if (cmd == DefaultKeys.Release
                            && !string.IsNullOrWhiteSpace(player.ChosenStat))
                        {
                            var statProp = player.Stats.GetType().GetProperty(player.ChosenStat);
                            if (statProp == null)
                                throw new Exception("Chosen stat not found");
                            if ((int)statProp.GetValue(player.Stats) > player.Stats.GetMinAttributeValue())
                            {
                                player.Stats.Experience += 1;
                                statProp.SetValue(player.Stats,
                                            (int)statProp.GetValue(player.Stats) - 1);
                            }
                        }
                    });
                });
        }
    }
}