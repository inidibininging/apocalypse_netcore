using System;
using System.Collections.Generic;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Services
{
    public class SetGameSectorNewDelegate : IStateSetService<string, IGameSectorLayerService>
    {
        private Func<Dictionary<string, IState<string, IGameSectorLayerService>>> GetDelegate { get; set; }

        public SetGameSectorNewDelegate(Func<Dictionary<string, IState<string, IGameSectorLayerService>>> getDelegate)
        {
            GetDelegate = getDelegate ?? throw new ArgumentNullException(nameof(getDelegate));
        }

        public bool Set(IState<string, IGameSectorLayerService> state, string identifier)
        {
            GetDelegate()?.Add(identifier, state);
            return true;
        }
    }
}