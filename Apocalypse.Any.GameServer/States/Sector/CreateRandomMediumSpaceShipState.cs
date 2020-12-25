using System;
using Apocalypse.Any.Core;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector
{
    /// <summary>
    /// Creates a random flying ship in the game
    /// </summary>
    public class CreateRandomMediumSpaceShipState: ICommand<IGameSectorLayerService>, IState<string, IGameSectorLayerService>
    {
        // private RandomMediumSpaceshipFactory RandomMediumSpaceshipFactory { get; set; }

        public bool CanExecute(IGameSectorLayerService parameters)
        {
            // if (parameters.CurrentStatus != GameSectorStatus.Running)
            //     return false;
            if (parameters.Factories.ImageDataFactory == null)
                return false;
            if (!parameters.Factories.ImageDataFactory.ContainsKey(nameof(RandomMediumSpaceshipFactory)))
                return false;
            return true;
        }

        public CreateRandomMediumSpaceShipState()
        {
        }

        public void Execute(IGameSectorLayerService parameters)
        {
            if (!CanExecute(parameters))
                return;
            var mediumSpaceShip = parameters.Factories.ImageDataFactory[nameof(RandomMediumSpaceshipFactory)].Create(parameters.SectorBoundaries);
            if (mediumSpaceShip == null)
                return;
            parameters.DataLayer.ImageData.Add(mediumSpaceShip);
            parameters.Messages.Add("Added medium space ship");
        }

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            Execute(machine.SharedContext);
        }
    }
}
