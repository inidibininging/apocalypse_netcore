using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Apocalypse.Any.Infrastructure.Server.Adapters.Redis
{

    public class GameSectorLayerWriterMechanic : GameSectorOwnerRedisMechanicBase
    {
        protected override void UpdateImplementation(IGameSectorsOwner entity) => Task.Run(() => WriteAsync(entity));

        private async Task WriteAsync(IGameSectorsOwner entity)
        {            
            // foreach (var sector in entity.GameSectorLayerServices)
            // {
            //     Write(sector.Value.SharedContext);
            // }
            using (var conn = await ConnectionMultiplexer.ConnectAsync($"{RedisHost}:{RedisPort}"))
            {
                var sub = conn.GetSubscriber();
                await sub.PublishAsync("State.Metrics", 
                    JsonConvert.SerializeObject(
                                entity
                                .GameSectorLayerServices
                                .Values
                                .Select(m => m.TimeLog)), 
                    CommandFlags.FireAndForget);
                await conn.GetDatabase().StringSetAsync($"World.Sectors",
                    JsonConvert.SerializeObject(
                    entity.GameSectorLayerServices.Keys.ToList()
                ));
            }
        }

        private void Write(IGameSectorLayerService gameSectorLayerService)
        {
            using (var conn = ConnectionMultiplexer.Connect($"{RedisHost}:{RedisPort}"))
            {
                
                conn.GetDatabase().StringSet($"{gameSectorLayerService.Tag}.Enemies", JsonConvert.SerializeObject(gameSectorLayerService.DataLayer.Enemies));
                conn.GetDatabase().StringSet($"{gameSectorLayerService.Tag}.Players", JsonConvert.SerializeObject(gameSectorLayerService.DataLayer.Players));
                conn.GetDatabase().StringSet($"{gameSectorLayerService.Tag}.Items", JsonConvert.SerializeObject(gameSectorLayerService.DataLayer.Items));
                conn.GetDatabase().StringSet($"{gameSectorLayerService.Tag}.GeneralCharacter", JsonConvert.SerializeObject(gameSectorLayerService.DataLayer.GeneralCharacter));
                conn.GetDatabase().StringSet($"{gameSectorLayerService.Tag}.ImageData", JsonConvert.SerializeObject(gameSectorLayerService.DataLayer.ImageData));
                conn.GetDatabase().StringSet($"{gameSectorLayerService.Tag}.Projectiles", JsonConvert.SerializeObject(gameSectorLayerService.DataLayer.Projectiles));
                conn.GetDatabase().StringSet($"{gameSectorLayerService.Tag}.MaxEnemies", gameSectorLayerService.MaxEnemies);
                conn.GetDatabase().StringSet($"{gameSectorLayerService.Tag}.MaxPlayers", gameSectorLayerService.MaxPlayers);
                conn.GetDatabase().StringSet($"{gameSectorLayerService.Tag}.SectorBoundaries", JsonConvert.SerializeObject(gameSectorLayerService.SectorBoundaries));
                conn.GetDatabase().StringSet($"{gameSectorLayerService.Tag}.Messages", JsonConvert.SerializeObject(gameSectorLayerService.Messages));
            }
        }
    }
}
