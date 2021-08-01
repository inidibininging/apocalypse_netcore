using Apocalypse.Any.Core;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector
{
    /// <summary>
    /// Creates a random planet in a random position
    /// </summary>
    public class CreateRandomPlanetCommand : ICommand<IGameSectorLayerService>, IState<string, IGameSectorLayerService>
    {
        private RandomPlanetFactory RandomPlanetFactory { get; set; }

        public bool CanExecute(IGameSectorLayerService parameters)
        {
            // if (parameters.CurrentStatus != GameSectorStatus.Running)
            //     return false;
            if (parameters.Factories.ImageDataFactory == null)
                return false;
            if (!parameters.Factories.ImageDataFactory.ContainsKey(nameof(RandomPlanetFactory)))
                return false;
            return true;
        }

        public CreateRandomPlanetCommand()
        {
        }

        public void Execute(IGameSectorLayerService parameters)
        {
            if (!CanExecute(parameters))
                return;
            var planet = parameters.Factories.ImageDataFactory[nameof(RandomPlanetFactory)].Create(parameters.SectorBoundaries);
            if (planet == null)
                return;
            parameters.DataLayer.ImageData.Add(planet);
            // parameters.Messages.Add("Added planet");
        }

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            Execute(machine.SharedContext);
	    // System.Console.WriteLine("creating planeeeeeet");
        }
    }
}
