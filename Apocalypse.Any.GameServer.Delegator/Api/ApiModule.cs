using System;
using Apocalypse.Any.GameServer.Delegator.Delegation;
using Nancy;

namespace Apocalypse.Any.GameServer.Delegator.Api
{
    public class ApiModule : NancyModule
    {
        private const string ApiPath = "/api/";

        public ApiModule()
        {
            Get(string.Format("{0}get", ApiPath), (req) => 
            {
                return CentralDelegationService.Instance.Messages;
            });
            Post(string.Format("{0}set", ApiPath), (req) => 
            {
                var id = this.Request.Body;
                long len = this.Request.Body.Length;
                byte[] data = new byte[len];

                return "The post";
            });
        }
    }
}
