using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.GameServer.States.Services
{
    public class SetGameSectorNewDelegate : IStateSetService<string, IGameSectorLayerService>
    {
        private Func<Dictionary<string, IState<string, IGameSectorLayerService>>> GetDelegate { get; set; }

        public SetGameSectorNewDelegate(Func<Dictionary<string, IState<string, IGameSectorLayerService>>> getDelegate)
        {
            if (getDelegate == null)
                throw new ArgumentNullException(nameof(getDelegate));
            GetDelegate = getDelegate;
        }

        public bool Set(IState<string, IGameSectorLayerService> state, string identifier)
        {
            var dictionary = GetDelegate();
            dictionary.Add(identifier, state);
            return true;
        }
    }
}