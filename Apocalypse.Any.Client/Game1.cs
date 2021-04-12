using System;
using System.Diagnostics;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Client.States;
using Apocalypse.Any.Client.States.Storage;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Client.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.Client
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public int SelectedStartScene { get; set; }
        public IStateMachine<string, INetworkGameScreen> GameContext { get; set; }
        
        public Process LocalServer { get; set; }
        
        private void InitLocalGameServer()
        {
            string apocalypseNetCorePath = ClientConfiguration.LocalServerPath;
            string pathToLocalServerConfig = $"{apocalypseNetCorePath}localserver_config.yaml";
            string pathToSyncServerConfig = $"{apocalypseNetCorePath}localserver_to_sync.yaml";
            string localGameServer = $"{apocalypseNetCorePath}Apocalypse.Any.GameServer/bin/Debug/net5.0/Apocalypse.Any.GameServer.dll";
            LocalServer = new Process
            {
                StartInfo = (new ProcessStartInfo()
                {
                    FileName = $"dotnet",
                    ArgumentList = {localGameServer, pathToLocalServerConfig, pathToSyncServerConfig}
                })
            };
            LocalServer.OutputDataReceived += LocalServerOutputDataReceived;
            LocalServer.Start();
        }

        private static void LocalServerOutputDataReceived(object sender, DataReceivedEventArgs args) => Console.WriteLine(args.Data);

        protected override void Dispose(bool disposing)
        {
            if(GameContext.SharedContext.Configuration.WithLocalServer){
                LocalServer.OutputDataReceived -= LocalServerOutputDataReceived;
                LocalServer.Kill();
                LocalServer.Dispose();
                LocalServer = null;
            }
            base.Dispose(disposing);
        }

        private GameClientConfiguration ClientConfiguration { get; }
        public Game1(GameClientConfiguration gameClientConfiguration) : base()
        {
            SelectStartScene(string.Empty);
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            var contextBuilder = new InMemoryGameScreenStorageFactory();
            GameContext = contextBuilder.BuildClientStateMachine(gameClientConfiguration);
            ClientConfiguration = gameClientConfiguration;
        }

        private void SelectStartScene(string selection)
        {
            SelectedStartScene = string.IsNullOrWhiteSpace(selection) ? 2 : selection.ElementAt(0) == '1' ? 1 : 2;
        }

        //private IUpdateableLite Duh;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            // TODO: Add your initialization logic here
            //Services.AddService(new DefaultBusInputRecordService());
            Services.AddService(ScreenService.Instance); //TODO: Need to change this -> referring to a new screen service , not to the  singleton instance
            this.IsMouseVisible = false;
            //Initialize Screen Dimension

            _graphics.PreferredBackBufferWidth = (int)System.MathF.Round(Services.GetService<ScreenService>().Resolution.X);
            _graphics.PreferredBackBufferHeight = (int)System.MathF.Round(Services.GetService<ScreenService>().Resolution.Y);
            //_graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            base.Initialize();


            GameContext.SharedContext = new NetworkGameScreen();
            GameContext.SharedContext.Initialize();
            if(ClientConfiguration.WithLocalServer)
                InitLocalGameServer();

            GameContext.GetService.Get(ClientGameScreenBook.Init).Handle(GameContext);
            ScreenService.Instance.Initialize(GameContext.SharedContext);


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //Load Screen Content
            ScreenService screenManager = Services.GetService<ScreenService>();
            screenManager.LoadContent(Content);
            screenManager.GraphicsDevice = GraphicsDevice;            
            screenManager.SpriteBatch = _spriteBatch;
        }

        public int LastMesageCount { get; set; }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
                this.EndRun();
            }
#endif
            //Duh?.Update(gameTime);
            ScreenService.Instance.Update(gameTime);
            
            GameContext.GetService.Get(ClientGameScreenBook.Update).Handle(GameContext);
            GameContext.SharedContext.Messages.Clear();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0, 0, 0));

            // TODO: Add your drawing code here

            //_spriteBatch.Begin();

            //A screen service should give you GameScreens and not manage how they work.
            ScreenService.Instance.Draw(_spriteBatch);

            //_spriteBatch.End();
            //TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            var screenManager = Services.GetService<ScreenService>();
            screenManager.UnloadContent();
            base.UnloadContent();
        }

    }
}
