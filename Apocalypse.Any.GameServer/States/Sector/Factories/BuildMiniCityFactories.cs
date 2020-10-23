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
using Apocalypse.Any.Constants;

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
        private int ImagePath { get; }

        public BuildMiniCityFactories(RectangularFrameGeneratorService frameGeneratorService, int imagePath)
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


            var centerFrames = FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 1, 6, 5, 5);
            var horizontalFramesChunkA = FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 5, 0, 0);
            
            var verticalFramesChunkA = FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 6, 1, 1);
            var verticalFramesChunkB = FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 2, 2, 2);
            
            var finalVerticalFramesChunk = verticalFramesChunkA
                                            .Union(verticalFramesChunkB.Select(kv => kv))
                                            .ToDictionary(kv => kv.Key, kv => kv.Value);
            
            var buildingTops = FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 3, 6, 2, 2);            
            var buildingDownsA = FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 6, 3, 4);
            var buildingDownsB = FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 5, 4, 0);
            var buildingDownsC = FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 0, 5, 5);

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

