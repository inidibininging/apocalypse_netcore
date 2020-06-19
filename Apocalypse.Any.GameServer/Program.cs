using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.GameServer.GameInstance;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.JsonAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.MsgPackAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.YamlAdapter;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Apocalypse.Any.GameServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //            var config = new GameServerConfiguration()
            //            {
            //                MaxEnemies = 10,
            //                MaxPlayers = 4,
            //                RedisHost = "localhost",
            //                RedisPort = 6379,
            //                ServerIp = "127.0.0.1",
            //                ServerPort = 8080,
            //                ServerPeerName = "asteroid",
            //                ServerUpdateInSeconds = 0.01f,
            //                StartingSector = "hub",
            //                BuildOperation = ServerGameSectorNewBook.BuildDefaultSectorState,
            //                RunOperation = ServerGameSectorNewBook.RunAsDefaultSector,
            //                StartupScript = "apocalypse.echse"
            ////                StartupScript = @":BuildSector
            ////                     !CreateRandomMediumSpaceShipState 
            ////                     !CreateRandomMediumSpaceShipState 
            ////                     !CreateRandomMediumSpaceShipState 
            ////                     !CreateRandomMediumSpaceShipState 
            ////                     !CreateRandomMediumSpaceShipState 
            ////                     !CreateRandomMediumSpaceShipState 
            ////                     !CreateRandomMediumSpaceShipState 
            ////                     !CreateRandomMediumSpaceShipState 
            ////                     !CreateRandomFogCommand 
            ////                     !CreateRandomFogCommand 
            ////                     !CreateRandomFogCommand 
            ////                     !CreateRandomFogCommand 
            ////                     !CreateRandomFogCommand 
            ////                     !CreateRandomFogCommand 
            ////                     !CreateRandomFogCommand 
            ////                     !CreateRandomPlanetState 
            ////                     !CreateRandomPlanetState 
            ////                     !CreateRandomPlanetState 
            ////",
            //            };

            //            File.WriteAllText(
            //                "startup.json",
            //                JsonConvert.SerializeObject(config)
            //            );

            var yamler = new YamlSerializerAdapter();
            var jsonler = new JsonSerializerAdapter();
            var msgler = new MsgPackSerializerAdapter();

            var config = yamler.DeserializeObject<GameServerConfiguration>(File.ReadAllText(args[0]));
            
            //File.WriteAllText(args[0] , yamler.Serialize(config));

            //            File.WriteAllText(
            //                "startup.yaml",
            //                yamler.Serialize(config)
            //            );
            //return;

            var world = new WorldGame(config);
            while (true)
            {
                world.Update(null);
            }
        }
    }
}