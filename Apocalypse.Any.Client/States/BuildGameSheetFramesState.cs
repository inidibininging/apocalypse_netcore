using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Client.Services;
using States.Core.Infrastructure.Services;
using System;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Client.States
{
    public class BuildGameSheetFramesState : IState<string, INetworkGameScreen>
    {
        public RectangularFrameGeneratorService FrameGeneratorService { get; set; }

        public BuildGameSheetFramesState(RectangularFrameGeneratorService frameGeneratorService)
        {
            FrameGeneratorService = frameGeneratorService ?? throw new ArgumentNullException(nameof(frameGeneratorService));
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(BuildGameSheetFramesState));
            machine.SharedContext.GameSheet.Frames = FrameGeneratorService.GenerateGameSheetAtlas();

            //foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("player", 32, 32, 0, 3, 7, 7))
            //    machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.PlayerFrame, 32, 32, 0, 4, 0, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.PlayerFrame, 32, 32, 0, 3, 4, 4))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.EnemyFrame, 32, 32, 0, 8, 0, 1))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MineralFrame, 32, 32, 0, 8, 2, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.AsteroidFrame, 32, 32, 0, 8, 2, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.ProjectileFrame, 32, 32, 4, 6, 7, 7))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.ExplosionFrame, 32, 32, 0, 3, 8, 8))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.FaceFrame, 32, 32, 0, 4, 0, 2))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.FaceFrame, 32, 32, 0, 3, 3, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
            
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.DialogueFrame, 32, 32, 0, 0, 0, 4))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            //@todo exploding diarrhea
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.ThrustFrame, 32, 32, 4, 8, 8, 8))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.ThrustFrame, 32, 32, 0, 8, 9, 9))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.ThrustFrame, 32, 32, 0, 0, 10, 10))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.ItemFrame, 32, 32, 0, 7, 0, 5))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.PlanetFrame, 32, 32, 0, 7, 6, 6))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.RandomPlanetFrame0, 128, 128, 0, 7, 0, 7))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.RandomPlanetFrame1, 128, 128, 0, 7, 0, 7))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.RandomPlanetFrame2, 128, 128, 0, 7, 0, 7))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MediumShipFrame, 128, 128, 0, 4, 0, 4))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
                
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.FogFrame, 512, 512, 0, 2, 0, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.HUDFrame, 32, 32, 0, 8, 0, 8))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 1, 6, 5, 5))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath,32, 32, 0, 5, 0, 0))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 6, 1, 1))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 2, 2, 2))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 3, 6, 2, 2))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 6, 3, 4))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 5, 4, 0))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas(ImagePaths.MiniCityImagePath, 32, 32, 0, 0, 5, 5))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);


        }
    }
}