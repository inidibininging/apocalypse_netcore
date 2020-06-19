using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.GameServer.States.Services
{
    public class NewGameSectorNewDelegate : IStateNewService<string, IGameSectorLayerService>
    {
        private Func<Dictionary<string, IState<string, IGameSectorLayerService>>> GetDelegate { get; set; }

        public NewGameSectorNewDelegate(Func<Dictionary<string, IState<string, IGameSectorLayerService>>> getDelegate)
        {
            if (getDelegate == null)
                throw new ArgumentNullException(nameof(getDelegate));
            GetDelegate = getDelegate;
        }

        public string New(IState<string, IGameSectorLayerService> state)
        {
            var dictionary = GetDelegate();
            var identifier = Guid.NewGuid().ToString();
            dictionary.Add(identifier, state);
            return identifier;
        }

        public string New(string identifier, IState<string, IGameSectorLayerService> state)
        {
            var dictionary = GetDelegate();
            dictionary.Add(identifier, state);
            return identifier;
        }
    }
}