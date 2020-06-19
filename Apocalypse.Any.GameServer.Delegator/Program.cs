using System;
using Apocalypse.Any.GameServer.Delegator.Delegation;

namespace Apocalypse.Any.GameServer.Delegator
{
    class Program
    {
        static void Main(string[] args)
        {
             //TODO:insert delegation config here through args
            CentralDelegationService.PeerName = "asteroid";
            CentralDelegationService.ServerIp = "127.0.0.1";
            CentralDelegationService.ServerPort = 8080;
            CentralDelegationService.Username = "foo3";
            CentralDelegationService.Password = "12345";


            //var host = new Nancy.NancyContext.Self.NancyHost(new Uri("http://localhost:8081"));
            //host.Start();

            Console.WriteLine("Server started!");
            Console.ReadKey();
            //host.Stop();
        }
    }
}
