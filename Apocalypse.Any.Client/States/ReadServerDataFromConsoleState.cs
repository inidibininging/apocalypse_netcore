using System;
using Apocalypse.Any.Client.Screens;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    ///     Reads console user input and passes it to the connection variables
    /// </summary>
    public class ReadServerDataFromConsoleState : IState<string, INetworkGameScreen>
    {
        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            Console.WriteLine("Server IP:");
            machine.SharedContext.Configuration.ServerIp = Console.ReadLine();

            Console.WriteLine("Port:");
            var port = 8080;
            int.TryParse(Console.ReadLine(), out port);
            machine.SharedContext.Configuration.ServerPort = port;

            Console.WriteLine("Enter Player Name:");
            machine.SharedContext.Configuration.User.Username = Console.ReadLine();

            Console.WriteLine("Enter Password:");
            machine.SharedContext.Configuration.User.Password = Console.ReadLine();
        }
    }
}