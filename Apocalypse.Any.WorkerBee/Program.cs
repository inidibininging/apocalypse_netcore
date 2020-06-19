using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
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
            var jsonler = typeof(JsonSerializerAdapter);
            var msgler = typeof(MsgPackSerializerAdapter);
            var config = yamler.DeserializeObject<GameClientConfiguration>(File.ReadAllText(args[0]));

            var dataLayerWorker = new DataLayerWorker<
                                        PlayerSpaceship,
                                        EnemySpaceship,
                                        Item,
                                        Projectile,
                                        CharacterEntity,
                                        CharacterEntity,
                                        ImageData>(config);
            while(true)
                dataLayerWorker.ProcessIncomingMessages();
        }
    }
}
