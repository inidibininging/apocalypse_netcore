using Apocalypse.Any.Client.Services;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Factories
{
    public class BuildMiniCityFactories : IState<string, IGameSectorLayerService>
    {
        public const string StreetCenterImageDataFactory = nameof(StreetCenterImageDataFactory);
        public const string HorizontalImageDataFactory = nameof(HorizontalImageDataFactory);
        public const string VerticalImageDataFactory = nameof(VerticalImageDataFactory);
        public const string BuildingTopDataFactory = nameof(BuildingTopDataFactory);
        public const string BuildingDataFactory = nameof(BuildingDataFactory);

        private RectangularFrameGeneratorService FrameGeneratorService { get; }
        private string ImagePath { get; }

        public BuildMiniCityFactories(RectangularFrameGeneratorService frameGeneratorService, string imagePath)
        {
            FrameGeneratorService = frameGeneratorService;
            ImagePath = imagePath;
        }
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (machine.SharedContext == null)
            {
                machine.SharedContext.Messages.Add("game sector is not available");
                return;
            }


            var centerFrames = FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 0, 0, 1, 1);
            var horizontalFramesChunkA = FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 0, 5, 0, 0);
            var verticalFramesChunkA = FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 3, 5, 1, 1);
            var verticalFramesChunkB = FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 0, 5, 2, 2);
            
            var finalVerticalFramesChunk = verticalFramesChunkA
                                            .Union(verticalFramesChunkB.Select(kv => kv))
                                            .ToDictionary(kv => kv.Key, kv => kv.Value);
            
            var buildingTops = FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 1, 4, 3, 3);            
            var buildingDownsA = FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 5, 5, 3, 3);
            var buildingDownsB = FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 0, 5, 4, 5);
            var buildingDownsC = FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 0, 1, 6, 6);

            var finalBuildingDowns = buildingDownsA
                                            .Union(buildingDownsB.Select(kv => kv))
                                            .Union(buildingDownsC.Select(kv => kv))
                                            .ToDictionary(kv => kv.Key, kv => kv.Value);

            var center = new RandomTilesetPartFactory(ImagePath, centerFrames.Select(kv => kv.Key).ToList());
            var horizontal = new RandomTilesetPartFactory(ImagePath, horizontalFramesChunkA.Select(kv => kv.Key).ToList());
            var vertical = new RandomTilesetPartFactory(ImagePath, finalVerticalFramesChunk.Select(kv => kv.Key).ToList());
            var buildingTop = new RandomTilesetPartFactory(ImagePath, buildingTops.Select(kv => kv.Key).ToList());
            var buildingDown = new RandomTilesetPartFactory(ImagePath, finalBuildingDowns.Select(kv => kv.Key).ToList());

            machine.SharedContext.Factories.ImageDataFactory.Add(StreetCenterImageDataFactory, center);
            machine.SharedContext.Factories.ImageDataFactory.Add(HorizontalImageDataFactory, horizontal);
            machine.SharedContext.Factories.ImageDataFactory.Add(VerticalImageDataFactory, vertical);
            machine.SharedContext.Factories.ImageDataFactory.Add(BuildingTopDataFactory, buildingTop);
            machine.SharedContext.Factories.ImageDataFactory.Add(BuildingDataFactory, buildingDown);
        }

    }
}

