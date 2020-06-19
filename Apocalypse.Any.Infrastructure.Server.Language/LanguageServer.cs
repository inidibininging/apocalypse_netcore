using System;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class LanguageServer : IState<string, IGameSectorLayerService>
    {
        public LanguageServer()
        {
            
        }
        //TODO
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            throw new NotImplementedException();
        }
    }
}
