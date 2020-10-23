using Apocalypse.Any.Client.GameObjects.Scene;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Client.Services;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Client.Model;
using Microsoft.Xna.Framework.Graphics;
using States.Core.Infrastructure.Services;
using System;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Client.States
{
    public class BuildClientSideState : IState<string, INetworkGameScreen>
    {
        public bool Initialized { get; private set; }
        private SpaceBackgroundElementsConfiguration SpaceBackgroundConfiguration { get; set; }

        private RectangularFrameGeneratorService FrameGeneratorService { get; set; }

        public BuildClientSideState(RectangularFrameGeneratorService frameGeneratorService, SpaceBackgroundElementsConfiguration spaceBackgroundElementsConfiguration)
        {
            FrameGeneratorService = frameGeneratorService ?? throw new ArgumentNullException(nameof(frameGeneratorService));
            SpaceBackgroundConfiguration = spaceBackgroundElementsConfiguration ?? throw new ArgumentNullException(nameof(spaceBackgroundElementsConfiguration));
        }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(BuildClientSideState));
            if (Initialized)
                return;

            machine.SharedContext.Messages.Add($"added {nameof(RectangleCollisionDetectionService)}");
            ScreenService.Instance.Collisions = new RectangleCollisionDetectionService();

            ScreenService.Instance.DefaultScreenCamera = new Core.Camera.TopDownCamera(new Viewport(ScreenService.Instance.GraphicsDevice.Viewport.Bounds));
            machine.SharedContext.Messages.Add($"added DefaultScreenCamera");

            BuildBackground(machine);
            machine.SharedContext.Messages.Add($"Built background");

            BuildLogo(machine);
            BuildGameOver(machine);

            machine.SharedContext.LoginSendResult = Lidgren.Network.NetSendResult.FailedNotConnected;
            Initialized = true;
        }

        private void BuildLogo(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.ContainsKey("logo"))
                return;
            var logo = new Image() { Path = ImagePaths.apocalypse_logo };            
            machine.SharedContext.Add("logo",logo);
        }

        private void BuildGameOver(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.ContainsKey("game_over"))
                return;
            var gameOver = new Image() { Path = ImagePaths.game_over_glitch };
            gameOver.Alpha.Alpha = 0;
            machine.SharedContext.Add("game_over", gameOver);
        }

        private void BuildBackground(IStateMachine<string, INetworkGameScreen> machine)
        {
            var asteroidField = new RandomAsteroidField(SpaceBackgroundConfiguration.AsteroidsCount);
            asteroidField.Initialize();
            machine.SharedContext.Add(nameof(RandomAsteroidField), asteroidField);

            var debrisField = new RandomDebrisField(SpaceBackgroundConfiguration.DebrisFieldCount);
            debrisField.Initialize();
            machine.SharedContext.Add(nameof(RandomDebrisField), debrisField);

            var starField = new StarField(SpaceBackgroundConfiguration.StarsFieldCount);
            starField.Initialize();
            machine.SharedContext.Add(nameof(StarField), starField);
        }
    }
}