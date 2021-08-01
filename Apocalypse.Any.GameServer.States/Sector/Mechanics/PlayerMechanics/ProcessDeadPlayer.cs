using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class ProcessDeadPlayer : IState<string, IGameSectorLayerService>
    {

        /// <summary>
        /// Used for creating the default values of a character
        /// </summary>
        public CharacterSheetFactory CharacterSheetFactory { get; set; } = new CharacterSheetFactory();
        public ProcessDeadPlayer()
        {

        }
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
                        if(cmd == DefaultKeys.Continue && player.Stats.Health <= player.Stats.GetMinAttributeValue())
                        {
                            
                            var newExp = player.Stats.Experience - player.Stats.Kills * player.Stats.Attack; //lol have fun

                            if(player.Stats.Experience < player.Stats.GetMinAttributeValue())
                            {
                                player.Stats.Aura += newExp;
                                player.Stats.Experience = 0;
                            }
                            else
                            {
                                player.Stats.Experience -= newExp;
                            }                            
                            player.Stats.Health = CharacterSheetFactory.GetDefaultStartSheet().Health;
                            player.CurrentImage.Position.X = machine.SharedContext.SectorBoundaries.MinSectorX;
                            player.CurrentImage.Position.Y = machine.SharedContext.SectorBoundaries.MinSectorY;
                        }
                    });
                 });
        }
    }
}
