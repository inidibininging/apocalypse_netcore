using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.GameServer.Delegator.Delegation
{
    public class CentralDelegationService
    {
        public static string PeerName { get; set; }
        public static string ServerIp { get; set; }
        public static int ServerPort { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }


        private static CentralDelegationService _instance;
        public static CentralDelegationService Instance
        {
            get
            {
                return _instance ?? (_instance = new CentralDelegationService(PeerName, ServerIp,ServerPort, Username,Password, _instance.SerializationAdapter));
            }
        }
        private UserDataDelegationService DelegationService { get; set; }
        public string Messages {
            get
            {
                string finalResult = string.Empty;
                if (DelegationService.Messages.Count == 0)
                    return "empty";

                DelegationService.Messages.TryTake(out finalResult);
                return finalResult;
            }
        }

        public IByteArraySerializationAdapter SerializationAdapter { get; }

        private CentralDelegationService(
            string peername,
            string ip,
            int port,
            string username,
            string password,
            IByteArraySerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
            DelegationService = new UserDataDelegationService(serializationAdapter);
            DelegationService.TryLogin(peername, ip, port, username, password);
        }

        public byte[] Feed()
        {
            return DelegationService.Feed();
        }
    }
}
