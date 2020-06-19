using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.YamlAdapter;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Apocalypse.Any.CLI
{
    internal class Program
    {
        private static bool ContinueCLI = true;

        private static void Main(string[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            
            System.Console.WriteLine("Welcome to Apocalypse.Any.CLI");
            System.Console.WriteLine("Server IP:");
            var serverIp = System.Console.ReadLine();
            System.Console.WriteLine("Server Port:");
            var serverPort = System.Console.ReadLine();

            //TODO:Load the cli server connector serializer needed for this. For now it shall be yaml
            var connector = new CLIServerConnector(new YamlSerializerAdapter()) { ServerIp = string.IsNullOrWhiteSpace(serverIp) ? "127.0.0.1" : serverIp, ServerPort = 8080 };
            connector.User = new UserData() { Username = "admin", Password = "12345" };
            connector.Initialize();
            System.Console.WriteLine("Connected");
            System.Console.WriteLine("Select one of the following commands and press Enter to execute it on server");
            
            System.Console.WriteLine("1. CreateEnemySpaceshipState");
            System.Console.WriteLine("2. CreateRandomPlanetState");
            System.Console.WriteLine("3. CurrentGameSectorInfo");
            System.Console.WriteLine("-- OR TYPE WHAT YOU WANT --");
            typeof(ServerGameSectorNewBook)
                .GetAllPublicConstantValues<string>()
                .ForEach(consta => Console.WriteLine(consta));

            System.Console.WriteLine("exit");

            while (ContinueCLI)
            {
                var command = Console.ReadLine();
//                 var scriptCommand = $@"
// :a Mod #{command} +0 Speed
// :b Mod #{command} +15 Speed
// :c !a Wait +5 Seconds !b
// :d !c Wait +10 Seconds !c";
//                 command = scriptCommand;
                // if (command == "1"){
                //     connector.Commands.Enqueue(ServerGameSectorNewBook.CreateEnemySpaceshipState);
                //     continue;
                // }
                // if (command == "2"){
                //     connector.Commands.Enqueue(ServerGameSectorNewBook.CreateRandomPlanetState);
                //     continue;
                // }
                // if (command == "3"){
                //     connector?.Data?.Images?.ForEach(img => Console.WriteLine($"{img.SelectedFrame} x:{img.Position.X} y:{img.Position.Y}"));
                //     continue;
                // }
                if (command == "skip")
                {
                    connector.Update();
                    continue;
                }
                if (command == "exit")
                {
                    Console.WriteLine("Bye!");
                    ContinueCLI = false;
                    break;
                }

                if(!string.IsNullOrWhiteSpace(command)){
                    for(var i = 0; i < 1;i++){
                            connector.Commands.Enqueue(command);
                            Console.WriteLine($"sent: {command}");
                            connector.Update();
                            connector.Commands.Clear();
                    }
                }

                // Task.Delay(connector.SecondsToNextLoginTry).Wait();
            }

            //console.WaitForBufferComplete();
        }

        public static StringBuilder CurrentBuffer { get; set; } = new StringBuilder();
        public static bool Flush { get; set; }

        private static ConsoleKey LastKey = ConsoleKey.Enter;
        private static int Repeat = 0;

        // private static void Console_OnKeyPress(KeyPressEventArgs e)
        // {
        //     if (LastKey == e.Key)
        //     {
        //         Repeat++;
        //         return;
        //     }

        //     if (Repeat > 1)
        //     {
        //         LastKey = ConsoleKey.Enter;
        //         Repeat = 0;
        //     }

        //     if (e.Key == ConsoleKey.Enter)
        //     {
        //         Flush = true;
        //         return;
        //     }

        //     var key = e.Key.ToString("f");
        //     CurrentBuffer.Append(key);
        //     LastKey = e.Key;
            
        // }
    }
}