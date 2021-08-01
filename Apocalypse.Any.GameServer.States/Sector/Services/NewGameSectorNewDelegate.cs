using System;
using System.Collections.Generic;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Services
{
    public class NewGameSectorNewDelegate : IStateNewService<string, IGameSectorLayerService>
    {
        private Func<Dictionary<string, IState<string, IGameSectorLayerService>>> GetDelegate { get; set; }

        public NewGameSectorNewDelegate(Func<Dictionary<string, IState<string, IGameSectorLayerService>>> getDelegate)
        {
            GetDelegate = getDelegate ?? throw new ArgumentNullException(nameof(getDelegate));
        }

        public string New(IState<string, IGameSectorLayerService> state)
        {
            var identifier = Guid.NewGuid().ToString();
            GetDelegate()?.Add(identifier, state);
            return identifier;
        }

        public string New(string identifier, IState<string, IGameSectorLayerService> state)
        {
            GetDelegate()?.Add(identifier, state);
            return identifier;
        }
    }
}