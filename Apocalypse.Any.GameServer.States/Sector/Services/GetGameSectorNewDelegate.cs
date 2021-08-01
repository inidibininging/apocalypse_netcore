using System;
using System.Collections.Generic;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Services
{
    public class GetGameSectorNewDelegate : IStateGetService<string, IGameSectorLayerService>
    {
        private Func<Dictionary<string, IState<string, IGameSectorLayerService>>> GetDelegate { get; set; }

        public GetGameSectorNewDelegate(Func<Dictionary<string, IState<string, IGameSectorLayerService>>> getDelegate)
        {
            if (getDelegate == null)
                throw new ArgumentNullException(nameof(getDelegate));
            GetDelegate = getDelegate;
        }
        
        public IEnumerable<string> States => GetDelegate().Keys;

        public IState<string, IGameSectorLayerService> Get(string identifier)
        {
            return GetDelegate()[identifier];
        }

        public bool HasState(string identifier)
        {
            return GetDelegate().ContainsKey(identifier);
        }

        public IStateGetService<string, IGameSectorLayerService> As<TSharedContextConverted>() where TSharedContextConverted : IGameSectorLayerService
        {
            return new GetGameSectorNewDelegate
            (
                GetDelegate
            );
        }
    }
}