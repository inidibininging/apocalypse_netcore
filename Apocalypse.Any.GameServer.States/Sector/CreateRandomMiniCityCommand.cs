using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector
{
    /// <summary>
    /// Creates or expands a city in the current sector
    /// </summary>
    public class CreateRandomMiniCityCommand : ICommand<IGameSectorLayerService>, IState<string, IGameSectorLayerService>
    {
        private RandomMiniCityFactory MiniCityFactory { get; set; }

        public bool CanExecute(IGameSectorLayerService parameters)
        {
            // if (parameters.CurrentStatus != GameSectorStatus.Running)
            //     return false;
            if (parameters.Factories.ImageDataFactory == null)
                return false;
            if (!parameters.Factories.ImageDataFactory.ContainsKey(Factories.BuildMiniCityFactories.StreetCenterImageDataFactory) &&
                !parameters.Factories.ImageDataFactory.ContainsKey(Factories.BuildMiniCityFactories.HorizontalImageDataFactory) &&
                !parameters.Factories.ImageDataFactory.ContainsKey(Factories.BuildMiniCityFactories.VerticalImageDataFactory) &&
                !parameters.Factories.ImageDataFactory.ContainsKey(Factories.BuildMiniCityFactories.BuildingTopDataFactory) &&
                !parameters.Factories.ImageDataFactory.ContainsKey(Factories.BuildMiniCityFactories.BuildingDataFactory))
                return false;
            return true;
        }

        public void Execute(IGameSectorLayerService parameters)
        {
            if (!CanExecute(parameters))
                return;
            if(MiniCityFactory == null)            
                MiniCityFactory = new RandomMiniCityFactory(parameters.Factories.ImageDataFactory[Factories.BuildMiniCityFactories.StreetCenterImageDataFactory],
                                                                        parameters.Factories.ImageDataFactory[Factories.BuildMiniCityFactories.HorizontalImageDataFactory],
                                                                        parameters.Factories.ImageDataFactory[Factories.BuildMiniCityFactories.VerticalImageDataFactory],
                                                                        parameters.Factories.ImageDataFactory[Factories.BuildMiniCityFactories.BuildingDataFactory],
                                                                        parameters.Factories.ImageDataFactory[Factories.BuildMiniCityFactories.BuildingTopDataFactory],
                                                                        50, 50, 50, 50);

            
            foreach (var cityPart in MiniCityFactory.Create(parameters.SectorBoundaries).ToList())
                parameters.DataLayer.ImageData.Add(cityPart);

            
        }

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            Execute(machine.SharedContext);
        }
    }
}
