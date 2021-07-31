using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.GameServer.GameInstance;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.JsonAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.MsgPackAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.YamlAdapter;
using System;
using System.IO;

namespace Apocalypse.Any.GameServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var yamler = new YamlSerializerAdapter();
            var jsonler = new JsonSerializerAdapter();
            var msgler = new MsgPackByteArraySerializerAdapter();
            
            var config = yamler.DeserializeObject<GameServerConfiguration>(File.ReadAllText(args[0]));
            GameClientConfiguration possibleSync = null;

            Console.WriteLine($"args length: {args.Length}");

            if(args.Length > 1)
                possibleSync = yamler.DeserializeObject<GameClientConfiguration>(File.ReadAllText(args[1]));

            var world = new WorldGame(config, possibleSync);
            while (true)
            {
                world.Update(null);
            }
        }
    }
}