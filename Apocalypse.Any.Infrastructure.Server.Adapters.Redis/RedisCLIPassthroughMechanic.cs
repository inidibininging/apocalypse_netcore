using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Apocalypse.Any.Infrastructure.Server.Adapters.Redis
{
    public class RedisCLIPassthroughMechanic : GameSectorOwnerRedisMechanicBase
    {

        public IUserAuthenticationService AuthenticationService { get; set; }
        public RedisCLIPassthroughMechanic(IUserAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        private async Task ReadAsync(IGameSectorsOwner entity)
        {
            using (var conn = await ConnectionMultiplexer.ConnectAsync($"{RedisHost}:{RedisPort}"))
            {
                var content = await conn.GetDatabase().StringGetAsync("World.CLI");
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var gameStateDataCommand = entity.GameSectorLayerServices.Values.SelectMany(s => s.SharedContext.DataLayer.Players.Select(plyr => plyr.LoginToken))
                            .Select(loginToken => AuthenticationService.GetByLoginTokenHack(loginToken))
                            .Where(user => (user.Roles & UserDataRole.CanSendRemoteStateCommands) != 0)
                            .SelectMany(user => entity.GameSectorLayerServices.Values.Select(
                            gameSector => gameSector.SharedContext.IODataLayer.GetGameStateByLoginToken(user.LoginToken))).FirstOrDefault();
                    if(gameStateDataCommand != null){
                        gameStateDataCommand.Commands.AddRange(JsonConvert.DeserializeObject<List<string>>(content));
                        await conn.GetDatabase().StringSetAsync("World.CLI",JsonConvert.SerializeObject(new List<string>()));
                    }
                }
            }
        }

        protected override void UpdateImplementation(IGameSectorsOwner entity) => ReadAsync(entity).Wait();
    }
}
