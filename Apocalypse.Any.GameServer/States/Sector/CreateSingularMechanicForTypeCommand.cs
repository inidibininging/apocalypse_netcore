using Apocalypse.Any.Core;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using System;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class CreateSingularMechanicForTypeCommand<TMechanicType> : ICommand<IGameSectorLayerService>
    {
        public bool CanExecute(IGameSectorLayerService parameters)
        {
            return true;
        }

        public void Execute(IGameSectorLayerService parameters)
        {
            throw new NotImplementedException();
        }
    }
}