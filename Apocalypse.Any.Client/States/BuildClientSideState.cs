using System;
using Apocalypse.Any.Client.GameObjects.Scene;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Client.Services;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Core.Camera;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Client.Model;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    ///     Builds only client side effects and game objects (background etc..)
    /// </summary>
    public class BuildClientSideState : IState<string, INetworkGameScreen>
    {
        public BuildClientSideState(RectangularFrameGeneratorService frameGeneratorService,
            SpaceBackgroundElementsConfiguration spaceBackgroundElementsConfiguration)
        {
            FrameGeneratorService =
                frameGeneratorService ?? throw new ArgumentNullException(nameof(frameGeneratorService));
            SpaceBackgroundConfiguration = spaceBackgroundElementsConfiguration ??
                                           throw new ArgumentNullException(
                                               nameof(spaceBackgroundElementsConfiguration));
        }

        public bool Initialized { get; private set; }
        private SpaceBackgroundElementsConfiguration SpaceBackgroundConfiguration { get; }

        private RectangularFrameGeneratorService FrameGeneratorService { get; }

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(BuildClientSideState));
            if (Initialized)
                return;

            machine.SharedContext.Messages.Add($"added {nameof(RectangleCollisionDetectionService)}");
            ScreenService.Instance.Collisions = new RectangleCollisionDetectionService();

            ScreenService.Instance.DefaultScreenCamera =
                new TopDownCamera(new Viewport(ScreenService.Instance.GraphicsDevice.Viewport.Bounds));
            machine.SharedContext.Messages.Add("added DefaultScreenCamera");

            BuildBackground(machine);
            BuildSparkField(machine);
            BuildLogo(machine);
            BuildGameOver(machine);

            machine.SharedContext.LoginSendResult = NetSendResult.FailedNotConnected;
            Initialized = true;
        }

        private void BuildLogo(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.ContainsKey("logo"))
                return;
            var logo = new Image {Path = ImagePaths.apocalypse_logo};
            machine.SharedContext.Add("logo", logo);
        }

        private void BuildGameOver(IStateMachine<string, INetworkGameScreen> machine)
        {
            if (machine.SharedContext.ContainsKey("game_over"))
                return;
            var gameOver = new Image {Path = ImagePaths.game_over_glitch};
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
            machine.SharedContext.Messages.Add("Built background");
        }

        /// <summary>
        ///     Adds a spark field generator for the game. This is used for the projectiles in the game
        /// </summary>
        private void BuildSparkField(IStateMachine<string, INetworkGameScreen> machine)
        {
            var sparkField = new RandomSparkField(SpaceBackgroundConfiguration.StarsFieldCount);
            sparkField.Initialize();
            machine.SharedContext.Add(nameof(RandomSparkField), sparkField);
        }
    }
}