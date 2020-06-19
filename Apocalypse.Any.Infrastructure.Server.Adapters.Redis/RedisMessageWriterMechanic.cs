using System;
using System.Linq;
using System.Threading.Tasks;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Apocalypse.Any.Infrastructure.Server.Adapters.Redis
{
    public class RedisMessagePassthroughMechanic : GameSectorOwnerRedisMechanicBase
    {
        private async Task WriteAsync(IGameSectorsOwner entity)
        {
            using (var conn = await ConnectionMultiplexer.ConnectAsync($"{RedisHost}:{RedisPort}"))
            {
                var messages = entity.GameSectorLayerServices.Values.SelectMany(sector => sector.SharedContext.Messages).ToList();
                await conn.GetDatabase().StringSetAsync("World.Messages",JsonConvert.SerializeObject(messages));
            }
        }
        protected override void UpdateImplementation(IGameSectorsOwner entity) => Task.Run(() => WriteAsync(entity));
    }
}
