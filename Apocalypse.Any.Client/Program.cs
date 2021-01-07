using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.JsonAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.MsgPackAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.YamlAdapter;
using SharpYaml.Serialization;
using System;

namespace Apocalypse.Any.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var gameClientConfiguration = new GameClientConfiguration()
            //{
            //    User = new Domain.Common.Model.Network.UserData() { Username = "foo2", Password = "12345" },
            //    Screen = new Domain.Common.Model.Network.ScreenData() { ScreenWidth = 1024, ScreenHeight = 768 },
            //    ServerIp = "192.168.188.42",
            //    ServerPort = 8080,
            //    ServerPeerName = "asteroid",
            //    Background = new SpaceBackgroundElementsConfiguration()
            //    {
            //        AsteroidsCount = 300,
            //        DebrisFieldCount = 1500,
            //        StarsFieldCount = 2000
            //    }
            //};
            var yamler = new YamlSerializerAdapter();
            var jsonler = new JsonSerializerAdapter();
            var msgler = new MsgPackByteArraySerializerAdapter();
            //System.IO.File.WriteAllText("startup.yaml",yamler.Serialize(gameClientConfiguration));
            var gameConfig = System.IO.File.ReadAllText(args[0]);
            var gameClientConfiguration = yamler.DeserializeObject<GameClientConfiguration>(gameConfig);
            //return;

            Console.WriteLine(gameConfig);
            using (var game = new Game1(gameClientConfiguration))
                game.Run();
            
        }
    }
}