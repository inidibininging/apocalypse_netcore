using Apocalypse.Any.Domain.Server.Configuration.Model;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.GameServer.GameInstance;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.JsonAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.MsgPackAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.YamlAdapter;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Apocalypse.Any.GameServer
{
    internal class Program
    {
        private static GameServerConfiguration GameServerConfiguration;
        private static YamlSerializerAdapter yamler = new YamlSerializerAdapter();
        private static JsonSerializerAdapter jsonler = new JsonSerializerAdapter();
        private static MsgPackSerializerAdapter msgler = new MsgPackSerializerAdapter();

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No GameServerConfiguration location provided");
                return;
            }
            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"Confiugration location doesn't exist: {args[0]}");
                return;
            }
            if (!TryReadConfiguration(args[0]))
                GameServerConfiguration = GetDefaultGameServerConfiguration();

            var world = new WorldGame(GameServerConfiguration);

            File.WriteAllText(args[0], yamler.SerializeObject(GameServerConfiguration));

            while (true)
                world.Update(null); // in the beginning of the game, game time should be not provided in order to create one            
        }
        private static bool TryReadConfiguration(string configurationLocation)
        {
            try
            {
                GameServerConfiguration = yamler.DeserializeObject<GameServerConfiguration>(File.ReadAllText(configurationLocation));
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"There was an error reading the configuration file: {configurationLocation}");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static GameServerConfiguration GetDefaultGameServerConfiguration()
        {
            return new GameServerConfiguration()
            {
                SerializationAdapterType = "Apocalypse.Any.Infrastructure.Common.Services.Serializer.MsgPackAdapter.MsgPackSerializerAdapter",
                RedisPort = 6379,
                RedisHost = string.Empty,
                ServerIp = "127.0.0.1",
                ServerPort = 8080,
                ServerUpdateInSeconds = 0.0099999997764825821,
                StartingSector = "hub",
                ServerPeerName = "asteroid",
                SectorConfigurations = new List<GameSectorConfiguration>()
                {
                    new GameSectorConfiguration()
                    {
                        Tag = "hub",
                        BuildOperation = ServerGameSectorNewBook.BuildDefaultSectorState,
                        RunOperation = ServerGameSectorNewBook.RunAsDefaultSector,
                        MaxEnemies = 16,
                        MaxPlayers = 4,
                        StartupFunction = "Main",
                        StartupScript = @"H:\Code\apocalypse_github\apocalypse.echse",
                        SectorBoundaries = new SectorBoundary()
                        {
                            MaxSectorX = 16000,
                            MaxSectorY = 16000
                        }
                    }
                }
            };
        }
    }
}