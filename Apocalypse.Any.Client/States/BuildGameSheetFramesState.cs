using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Client.Services;
using States.Core.Infrastructure.Services;
using System;

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
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("player", 32, 32, 0, 4, 0, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("player", 32, 32, 0, 3, 4, 4))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("enemy", 32, 32, 0, 8, 0, 1))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("minerals", 32, 32, 0, 8, 2, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("asteroids", 32, 32, 0, 8, 2, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("projectile", 32, 32, 4, 6, 7, 7))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("explosion", 32, 32, 0, 3, 8, 8))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("faces", 32, 32, 0, 4, 0, 2))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("faces", 32, 32, 0, 3, 3, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            //@todo exploding diarrhea
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("thrust", 32, 32, 4, 8, 8, 8))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("thrust", 32, 32, 0, 8, 9, 9))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("thrust", 32, 32, 0, 0, 10, 10))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("item", 32, 32, 0, 7, 0, 5))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("planet", 32, 32, 0, 7, 6, 6))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("planetsRandom0", 128, 128, 0, 7, 0, 7))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("planetsRandom1", 128, 128, 0, 7, 0, 7))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("planetsRandom2", 128, 128, 0, 7, 0, 7))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("mediumShips_edit", 128, 128, 0, 4, 0, 4))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
                
            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("fog_edit", 512, 512, 0, 2, 0, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("hud_misc_edit", 32, 32, 0, 8, 0, 7))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 0, 0, 1, 1))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 0, 5, 0, 0))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 3, 5, 1, 1))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 0, 5, 2, 2))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 1, 4, 3, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 5, 5, 3, 3))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 0, 5, 4, 5))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);

            foreach (var currentFrame in FrameGeneratorService.GenerateGameSheetAtlas("miniCity", 32, 32, 0, 1, 6, 6))
                machine.SharedContext.GameSheet.Frames.Add(currentFrame.Key, currentFrame.Value);
        }
    }
}