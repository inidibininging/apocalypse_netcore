using System;
using Apocalypse.Any.Core;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class CreateRandomFogCommand: ICommand<IGameSectorLayerService>, IState<string, IGameSectorLayerService>
    {
        //private RandomFogFactory RandomMediumSpaceshipFactory { get; set; }

        public bool CanExecute(IGameSectorLayerService parameters)
        {
            if (parameters.CurrentStatus != GameSectorStatus.Running)
                return false;
            if (parameters.Factories.ImageDataFactory == null)
                return false;
            if (!parameters.Factories.ImageDataFactory.ContainsKey(nameof(RandomMediumSpaceshipFactory)))
                return false;
            return true;
        }

        public CreateRandomFogCommand()
        {
        }

        public void Execute(IGameSectorLayerService parameters)
        {
            if (!CanExecute(parameters))
                return;
            var mediumFog = parameters.Factories.ImageDataFactory[nameof(RandomFogFactory)].Create(parameters.SectorBoundaries);
            if (mediumFog == null)
                return;
            parameters.DataLayer.ImageData.Add(mediumFog);
            parameters.Messages.Add("Added fog");
        }

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            Execute(machine.SharedContext);
        }
    }
}
