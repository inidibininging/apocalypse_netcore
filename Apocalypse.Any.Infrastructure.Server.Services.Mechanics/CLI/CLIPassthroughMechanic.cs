using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Language;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Infrastructure.Server.Services.Data;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.CLI
{
    public class CLIPassthroughMechanic :
        ISingleUpdeatableGameSectorOwnerMechanic
    {
        public bool Active { get; set; } = true;
        private readonly IUserAuthenticationService UserAuthenticationService;
        private readonly Interpreter CommandInterpreter;
        public CLIPassthroughMechanic(IUserAuthenticationService userAuthenticationService, string runOperation)
        {
            UserAuthenticationService = userAuthenticationService ?? throw new ArgumentNullException(nameof(userAuthenticationService));
            CommandInterpreter = new Interpreter(runOperation);
        }

        private Dictionary<string, UserDataWithLoginToken> CachedUserDataWithLoginToken { get; set; } =
            new Dictionary<string, UserDataWithLoginToken>();

        private DateTime NextCacheFlush { get; set; } = DateTime.Now;
        private TimeSpan CacheFlushInterval { get; set; } = 5.Minutes();
        public IGameSectorsOwner Update(IGameSectorsOwner entity)
        {
            // flush cache every now and then (see CacheFlushInterval)
            if (DateTime.Now > NextCacheFlush)
            {
                CachedUserDataWithLoginToken.Clear();
                NextCacheFlush = DateTime.Now.Add(CacheFlushInterval);
            }
                
            //TODO: save this query somewhere (player <=> data for game sector owner)
            foreach (var gameStateDataCommand in entity.GameSectorLayerServices.Values.SelectMany(s => s.SharedContext.DataLayer.Players.Select(plyr => plyr.LoginToken))
                            .Select(loginToken =>
                            {
                                // cache user data, based on its login token. This should change in the future. The password is stored here !!!!
                                if(CachedUserDataWithLoginToken.ContainsKey(loginToken)) 
                                   return CachedUserDataWithLoginToken[loginToken];
                                CachedUserDataWithLoginToken[loginToken] =
                                    UserAuthenticationService.GetByLoginTokenHack(loginToken);
                                return CachedUserDataWithLoginToken[loginToken];
                            }).Where(user => user.Roles != null && user.Roles.ContainsKey(UserDataRoleSource.SyncServer) && user.Roles[UserDataRoleSource.SyncServer] == UserDataRole.CanSendRemoteStateCommands)
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
