using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.GameServer.States.Services
{
    public class GetGameSectorNewDelegate : IStateGetService<string, IGameSectorLayerService>
    {
        private Func<Dictionary<string, IState<string, IGameSectorLayerService>>> GetDelegate { get; set; }

        public GetGameSectorNewDelegate()
        {
            
        }
        public GetGameSectorNewDelegate(Func<Dictionary<string, IState<string, IGameSectorLayerService>>> getDelegate)
        {
            if (getDelegate == null)
                throw new ArgumentNullException(nameof(getDelegate));
            GetDelegate = getDelegate;
            
        }

        public IState<string, IGameSectorLayerService> Get(string identifier)
        {

            return GetDelegate()[identifier];
        }

        public bool HasState(string identifier)
        {
            return GetDelegate().ContainsKey(identifier);
        }
    }
}