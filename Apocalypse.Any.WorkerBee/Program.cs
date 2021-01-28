using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.GameServer.GameInstance;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.JsonAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.MsgPackAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.YamlAdapter;
using Apocalypse.Any.Infrastructure.Server.Worker;
using System;
using System.IO;

namespace Apocalypse.Any.WorkerBee
{
    class Program
    {
        static void Main(string[] args)
        {
            var yamler = new YamlSerializerAdapter();

            var clientConfig = yamler.DeserializeObject<GameClientConfiguration>(File.ReadAllText(args[0]));
            var serverConfig = yamler.DeserializeObject<GameServerConfiguration>(File.ReadAllText(args[1]));

            var world = new WorldGame(serverConfig, null);
            var dataLayerWorker = new SyncClient<
                                        PlayerSpaceship,
                                        EnemySpaceship,
                                        Item,
                                        Projectile,
                                        CharacterEntity,
                                        CharacterEntity,
                                        ImageData>(clientConfig);
             
            while (true)
            {
                world.Update(null);
                dataLayerWorker.ProcessIncomingMessages(null);
                
            }

            
                
        }
    }
}
