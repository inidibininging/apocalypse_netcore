using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Data;
using Apocalypse.Any.Infrastructure.Server.Language;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.CLI
{
    public class CLIPassthroughMechanic :
        ISingleUpdeatableGameSectorOwnerMechanic
    {
        private readonly IUserAuthenticationService UserAuthenticationService;
        private readonly Interpreter CommandInterpreter;
        public CLIPassthroughMechanic(IUserAuthenticationService userAuthenticationService)
        {
            UserAuthenticationService = userAuthenticationService ?? throw new ArgumentNullException(nameof(userAuthenticationService));
            CommandInterpreter = new Interpreter();
        }

        public IGameSectorsOwner Update(IGameSectorsOwner entity)
        {
            //TODO: save this query somewhere (player <=> data for game sector owner)
            foreach (var gameStateDataCommand in entity.GameSectorLayerServices.Values.SelectMany(s => s.SharedContext.DataLayer.Players.Select(plyr => plyr.LoginToken))
                            .Select(loginToken => UserAuthenticationService.GetByLoginTokenHack(loginToken))
                            .Where(user => (user.Roles & UserDataRole.CanSendRemoteStateCommands) != 0)
                            .SelectMany(user => entity.GameSectorLayerServices.Values.Select(
                            gameSector =>
                            {
                                try
                                {
                                    var gs = gameSector.SharedContext.IODataLayer.GetGameStateByLoginToken(user.LoginToken);
                                    return gs.Commands;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                return new List<string>();
                            })))
            {
                gameStateDataCommand.ForEach(cmd =>
                {
                    foreach (var gameSector in entity.GameSectorLayerServices.Values)
                    {
                        try{
                            gameSector.GetService.Get(cmd).Handle(gameSector);
                        }
                        catch(Exception ex){
                            CommandInterpreter.Context = gameSector;
                            CommandInterpreter.Run(cmd);
                            CommandInterpreter.Context = null;
                        }
                    }
                });
                gameStateDataCommand.Clear();
            }
            return entity;
        }
    }
}
