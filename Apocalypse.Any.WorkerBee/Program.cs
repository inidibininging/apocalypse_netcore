using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.GameServer.GameInstance;
using Apocalypse.Any.GameServer.Services;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.JsonAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.MsgPackAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.YamlAdapter;
using Apocalypse.Any.Infrastructure.Server.Worker;
using System;
using System.Collections.Generic;
using System.IO;

namespace Apocalypse.Any.WorkerBee
{
    internal static class Program
    {
        private static readonly IntToStringCommandTranslator Translator = new();
        private static readonly LoggerServiceFactory LoggerFactory = new();
        private static void Main(string[] args)
        {
            var yamler = new YamlSerializerAdapter();

            // var serverConfig = yamler.DeserializeObject<GameServerConfiguration>(File.ReadAllText(args[0]));
            var clientConfig = args.Length > 1 ? yamler.DeserializeObject<GameClientConfiguration>(File.ReadAllText(args[1])) : null;
            var syncClient = new SyncClient<PlayerSpaceship, EnemySpaceship, Item, Projectile, CharacterEntity, CharacterEntity, ImageData>(clientConfig, LoggerFactory.GetLogger());
            ConsoleKey nextConsoleKey;
            while ((nextConsoleKey = System.Console.ReadKey().Key) != ConsoleKey.Escape) {
                syncClient.ProcessIncomingMessages(new List<int>() { Translator.Translate(ToStringCommand(nextConsoleKey)) });
            }
        }
        private static string ToStringCommand(ConsoleKey key) {
            return key switch
            {
                ConsoleKey.W => DefaultKeys.Up,
                ConsoleKey.S => DefaultKeys.Down,
                ConsoleKey.D => DefaultKeys.Right,
                ConsoleKey.A => DefaultKeys.Left,
                ConsoleKey.Enter => DefaultKeys.Shoot,
                ConsoleKey.Backspace => DefaultKeys.Boost,
                _ => null,
            };
        }
    }
}
