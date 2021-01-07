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
           
            var yamler = new YamlSerializerAdapter();
            var jsonler = new JsonSerializerAdapter();
            var msgler = new MsgPackByteArraySerializerAdapter();

            var config = yamler.DeserializeObject<GameServerConfiguration>(File.ReadAllText(args[0]));            

            var world = new WorldGame(config);
            while (true)
            {
                world.Update(null);
            }
        }
    }
}