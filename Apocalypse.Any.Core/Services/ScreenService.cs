using Apocalypse.Any.Core.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Apocalypse.Any.Core.Services
{
    /// <summary>
    /// This service provides all functionality for loading and unloading screens.
    /// Furthermore, it is used now for violating design principles and bounding every object the singleton instance of this object.
    /// TODO: Decoupling of ScreenService => (WILL CHANGE IN THE FUTURE)
    /// This is not only a screen service but also the actual owner of all game objects => dangerous!!!
    /// </summary>
    public class ScreenService : IScreenService
    {
        private readonly Vector2 defaultScreenResolution = new Vector2(1920, 1080);
        public Vector2 Ratio { get { return Resolution / DefaultScreenResolution; } }
        public Vector2 DefaultScreenResolution { get { return defaultScreenResolution; } }

        public ScreenService()
        {
            Resolution = DefaultScreenResolution;
            //Start Resolution
            CurrentScreen = new GameScreen();
            Sounds = new WaveSoundService();
        }

        public void LoadContent(ContentManager content)
        {
            Content = content;//new ContentManager(content.ServiceProvider, "Content");
            Sounds.LoadContent(content);
            CurrentScreen.LoadContent(content);
        }

        public void UnloadContent()
        {
            CurrentScreen.UnloadContent();
            Sounds.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (!ChangingScreen)
            {
                //why restrict to three cameras when you can have a ton of them?
                //this obviously need a list and a way of interacting with other classes
                DefaultScreenCamera?.Update(gameTime);
                LeftScreenCamera?.Update(gameTime);
                RightScreenCamera?.Update(gameTime);
                CurrentScreen.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws all objects to the screen, if the screen is not in transition aka. changing
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!ChangingScreen)
            {
                CurrentScreen.Draw(spriteBatch);
            }
        }

        #region Properties

        // -.-
        private static ScreenService _instance;

        private IGameScreen CurrentScreen { get; set; }
        public Vector2 Resolution { private set; get; }
        public ContentManager Content { private set; get; }
        private IGameScreen DefaultScreen { get; }
        public GraphicsDevice GraphicsDevice;
        public SpriteBatch SpriteBatch;

        public Core.Camera.TopDownCamera DefaultScreenCamera { get; set; }
        public Core.Camera.TopDownCamera LeftScreenCamera { get; set; }
        public Core.Camera.TopDownCamera RightScreenCamera { get; set; }

        public static ScreenService Instance => _instance ?? (_instance = new ScreenService());
        public WaveSoundService Sounds { get; set; }
        public RectangleCollisionDetectionService Collisions { get; set; }

        #endregion Properties

        #region General Functions

        public bool ChangingScreen { get; private set; }

        public void ChangeScreen<T>() where T : GameScreen, new()
        {
            if (!ChangingScreen)
            {
                ChangingScreen = true;
                UnloadContent();
                CurrentScreen = Activator.CreateInstance<T>();
                LoadContent(Content);
                ChangingScreen = false;
            }
        }

        public void ChangeScreen<T>(T gameScreen) where T : IGameScreen
        {
            if (!ChangingScreen)
            {
                ChangingScreen = true;
                UnloadContent();
                CurrentScreen = gameScreen;
                LoadContent(Content);
                ChangingScreen = false;
            }
        }

        public void Initialize<T>(T gameScreen) where T : IGameScreen
        {
            Collisions = new RectangleCollisionDetectionService();
            DefaultScreenCamera = new Camera.TopDownCamera(new Viewport(GraphicsDevice.Viewport.Bounds));

            ChangeScreen(gameScreen);
            // CurrentScreen.Initialize();
        }

        public void Initialize<T>() where T : GameScreen, new()
        {
            ChangeScreen<T>();
            Collisions = new RectangleCollisionDetectionService();

            //@todo does this need an update, if the window changes in the future?
            GetLeftViewPort();
            GetRightViewPort();

            DefaultScreenCamera = new Camera.TopDownCamera(new Viewport(GraphicsDevice.Viewport.Bounds));
            // CurrentScreen.Initialize();
        }

        private void GetRightViewPort()
        {
            var rightScreenCameraViewport = new Viewport(GraphicsDevice.Viewport.Bounds);
            rightScreenCameraViewport.Width /= 2;
            rightScreenCameraViewport.X = LeftScreenCamera.CurrentViewport.Width;
            RightScreenCamera = new Camera.TopDownCamera(rightScreenCameraViewport);
        }

        private void GetLeftViewPort()
        {
            var leftScreenCameraViewport = new Viewport(GraphicsDevice.Viewport.Bounds);
            leftScreenCameraViewport.Width /= 2;
            LeftScreenCamera = new Camera.TopDownCamera(leftScreenCameraViewport);
        }

        public void Initialize()
        {
            if (CurrentScreen == null)
                return;
            CurrentScreen.Initialize();
            //throw new NotImplementedException();
        }

        #endregion General Functions
    }
}