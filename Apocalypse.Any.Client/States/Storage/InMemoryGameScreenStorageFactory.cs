using Apocalypse.Any.Client.GameObjects.Scene;
using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Client.Services;
using Apocalypse.Any.Client.Services.Creation;
using Apocalypse.Any.Client.States.Services;
using Apocalypse.Any.Client.States.UI;
using Apocalypse.Any.Client.States.UI.Character;
using Apocalypse.Any.Client.States.UI.Chat;
using Apocalypse.Any.Client.States.UI.Dialog;
using Apocalypse.Any.Client.States.UI.Info;
using Apocalypse.Any.Client.States.UI.Inventory;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Data;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using States.Core.Common;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Client.States.Storage
{
    public class InMemoryGameScreenStorageFactory
    {
        public IStateMachine<string, INetworkGameScreen> BuildClientStateMachine(GameClientConfiguration gameClientConfiguration)
        {
            var inMemoryStorage = new Dictionary<string, IState<string, INetworkGameScreen>>();

            var serializer = Activator.CreateInstance(gameClientConfiguration.SerializationAdapterType.LoadType(true, false)[0]) as ISerializationAdapter;

            inMemoryStorage.Add(ClientGameScreenBook.CheckLoginSent, new CheckLoginSentState());
            inMemoryStorage.Add(ClientGameScreenBook.CreateFetchDataIfNotExists, new CommandStateStateDelegate<string, INetworkGameScreen>((machine) =>
              {
                  try
                  {
                      return machine.GetService.Get(ClientGameScreenBook.FetchData);
                  }
                  catch (Exception ex)
                  {
                      machine.SharedContext.Messages.Add(ex.Message);
                      var identifier = machine.NewService.New(ClientGameScreenBook.FetchData, new FetchDataState(
                          new NetIncomingMessageBusService<NetClient>(machine.SharedContext.Client),
                          serializer,
                          new DeltaGameStateDataService()
                      ));
                      return machine.GetService.Get(ClientGameScreenBook.FetchData);
                  }
              }));

            inMemoryStorage.Add("BuildConfig", new CommandStateActionDelegate<string, INetworkGameScreen>((machine) =>
            {
                machine.SharedContext.Configuration = gameClientConfiguration;
            }));

            inMemoryStorage.Add(ClientGameScreenBook.FillWithDefaultServerDataState, new FillWithDefaultServerDataState());
            //inMemoryStorage.Add(ClientGameScreenBook.ReadServerDataFromConsole, new ReadServerDataFromConsoleState());
            inMemoryStorage.Add(ClientGameScreenBook.BuildClientSideState, new BuildClientSideState(
                new RectangularFrameGeneratorService(),
                gameClientConfiguration.Background));
            inMemoryStorage.Add(ClientGameScreenBook.BuildGameSheetFrames, new BuildGameSheetFramesState(new RectangularFrameGeneratorService()));

            inMemoryStorage.Add(ClientGameScreenBook.BuildCursor, new BuildCursorState());
            inMemoryStorage.Add(ClientGameScreenBook.BuildClient, new BuildClientState());

            //inMemoryStorage.Add(ClientGameScreenBook.BuildInputService, new BuildInputServiceState(null));                        

            inMemoryStorage.Add(ClientGameScreenBook.BuildDefaultInputService, new BuildDefaultInputServiceState());
            inMemoryStorage.Add(ClientGameScreenBook.Connect, new ConnectState());
            inMemoryStorage.Add(ClientGameScreenBook.Init, new RoutineState<string, INetworkGameScreen>()
            {
                Operations = new List<string>()
                {
                    "BuildConfig",
                    ClientGameScreenBook.BuildClientSideState,
                    ClientGameScreenBook.BuildGameSheetFrames,
                    ClientGameScreenBook.BuildClient,
                    ClientGameScreenBook.BuildCursor,
                    ClientGameScreenBook.BuildDefaultInputService,
                    ClientGameScreenBook.BuildCharacterWindow,
                    ClientGameScreenBook.BuildInfoWindow,
                    ClientGameScreenBook.BuildInventoryWindow,
                    nameof(BuildDialogWindowState),
                    //ClientGameScreenBook.ReadServerDataFromConsole,
                    // ClientGameScreenBook.FillWithDefaultServerDataState,
                    ClientGameScreenBook.Connect,
                    ClientGameScreenBook.CreateFetchDataIfNotExists,
                    ClientGameScreenBook.Login
                }
            });


            inMemoryStorage.Add("UpdateLogoPosition", new CommandStateActionDelegate<string, INetworkGameScreen>((machine) =>
            {
                var logo = machine.SharedContext.As<Image>("logo");
                if (logo == null)
                    return;
                if (machine.SharedContext.InventoryWindow == null)
                    return;
                if (machine.SharedContext.LastMetadataBag == null)
                    logo.Alpha.Alpha = 0.00f;
                else
                    logo.Alpha.Alpha = 0.25f;
                logo.Scale = new Vector2() { X = 0.25f, Y = 0.25f };
                logo.Position.X = machine.SharedContext.InventoryWindow.Position.X + 64;
                logo.Position.Y = machine.SharedContext.InventoryWindow.Position.Y + 232;
                logo.LayerDepth = DrawingPlainOrder.UI;
            }));

            inMemoryStorage.Add("UpdateGameOverPosition", new CommandStateActionDelegate<string, INetworkGameScreen>((machine) =>
            {
                if(machine.SharedContext.LastMetadataBag == null)                
                    return;                
                if (machine.SharedContext.LastMetadataBag.Stats == null)
                    return;

                var hideGameOver = machine.SharedContext.LastMetadataBag.Stats.Health > machine.SharedContext.LastMetadataBag.Stats.GetMinAttributeValue();
                
                var gameOver = machine.SharedContext.As<Image>("game_over");
                if (gameOver == null)
                    return;
                if (hideGameOver)
                {
                    gameOver.Alpha.Alpha = 0f;
                }
                else
                {
                    gameOver.Scale = new Vector2() { X = 1.75f, Y = 1.75f };
                    gameOver.Alpha.Alpha = 1.00f;
                    gameOver.LayerDepth = DrawingPlainOrder.Foreground;
                    gameOver.Position.X = ScreenService.Instance.DefaultScreenCamera.Position.X;
                    gameOver.Position.Y = ScreenService.Instance.DefaultScreenCamera.Position.Y;
                }                
            }));

            inMemoryStorage.Add("UpdateBackground", new CommandStateActionDelegate<string, INetworkGameScreen>((machine) =>
            {
                foreach (Star starToMove in machine.SharedContext
                                            .As<StarField>(nameof(StarField))?
                                            .StarsAvailable
                                            .Where(star => star.CannotDraw()))
                {
                    try
                    {

                        var minFactor = 1.5f; // need this for evading the randomisation starting values
                        
                        var directionX = Randomness.Instance.RollTheDice(5) ? 1 : -1;
                        var directionY = Randomness.Instance.RollTheDice(10) ? 1 : -1;
                        starToMove.Position.X = ScreenService.Instance.DefaultScreenCamera.Position.X + (Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.X) * ScreenService.Instance.DefaultScreenCamera.Zoom * directionX);
                        starToMove.Position.Y = ScreenService.Instance.DefaultScreenCamera.Position.Y + (Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.Y) * ScreenService.Instance.DefaultScreenCamera.Zoom * directionY);

                    }
                    catch (Exception) { }
                }

            }));

            inMemoryStorage.Add(nameof(NetworkInterpolationState), new NetworkInterpolationState(new DeltaGameStateDataService()));
            inMemoryStorage.Add(ClientGameScreenBook.Update, new RoutineState<string, INetworkGameScreen>()
            {
                Operations = new List<string>()
                {
                    ClientGameScreenBook.UpdateCursor,
                    ClientGameScreenBook.UpdateInput,
                    ClientGameScreenBook.FetchData,
                    
                    ClientGameScreenBook.UpdateImages,
                    ClientGameScreenBook.UpdateCharacterWindow,
                    ClientGameScreenBook.UpdateMetadataState,
                    ClientGameScreenBook.UpdateInventoryImages,
                    ClientGameScreenBook.UpdateScreen,
                    ClientGameScreenBook.UpdateCamera,
                    ClientGameScreenBook.UpdateInventoryWindow,
                    nameof(UpdateDialogWindowState),
                    nameof(UpdateInventoryItemHoverTextState),
                    nameof(UpdateDialogHoverTextState),
                    ClientGameScreenBook.UpdateInfoWindow,
                    ClientGameScreenBook.SendGameStateUpdateData,
                    nameof(NetworkInterpolationState),
                    "UpdateBackground",
                    "UpdateLogoPosition",
                    "UpdateGameOverPosition"
                }
            });

            inMemoryStorage.Add(ClientGameScreenBook.UpdateInput, new UpdateInputsState());
            inMemoryStorage.Add(ClientGameScreenBook.UpdateCursor, new UpdateCursorState());
            inMemoryStorage.Add(ClientGameScreenBook.UpdateImages, new UpdateImagesState());


            inMemoryStorage.Add(nameof(UpdateInventoryItemHoverTextState), new UpdateInventoryItemHoverTextState());

            //converts metadata to an object
            var dataConverter = new NetworkCommandDataConverterService(serializer);
            inMemoryStorage.Add(ClientGameScreenBook.UpdateMetadataState, new UpdateMetadataState(dataConverter));
            inMemoryStorage.Add(ClientGameScreenBook.UpdateScreen, new UpdateScreenState());
            inMemoryStorage.Add(ClientGameScreenBook.UpdateCamera, new UpdateCameraState());
            inMemoryStorage.Add(ClientGameScreenBook.SendGameStateUpdateData, new SendGameStateUpdateDataState(serializer));
            inMemoryStorage.Add(ClientGameScreenBook.Login, new LoginState(
                   gameClientConfiguration.User,
                   gameClientConfiguration.ServerIp,
                   gameClientConfiguration.ServerPort,
                   serializer));

            //TODO: Dummy sheet dictionary for adding the only image needed . duh. definitely a
            var dummyHudSheetToChangeInTheFuture = new Dictionary<string, Rectangle>();
            dummyHudSheetToChangeInTheFuture.Add("hud_misc_edit_0_0", new Rectangle(0, 0, 32, 32));

            inMemoryStorage.Add(ClientGameScreenBook.BuildInventoryWindow, new BuildInventoryWindowState(dummyHudSheetToChangeInTheFuture));
            inMemoryStorage.Add(ClientGameScreenBook.UpdateInventoryWindow, new UpdateInventoryWindowState());
            inMemoryStorage.Add(ClientGameScreenBook.UpdateInventoryImages, new UpdateInventoryImagesState(new PlayerInventoryDrawingFactory()));
            inMemoryStorage.Add(nameof(UpdateDialogHoverTextState), new UpdateDialogHoverTextState());
            inMemoryStorage.Add(ClientGameScreenBook.BuildChatWindow, new BuildChatWindowState());
            
            inMemoryStorage.Add(nameof(BuildDialogWindowState), new BuildDialogWindowState());
            inMemoryStorage.Add(nameof(UpdateDialogWindowState), new UpdateDialogWindowState());

            inMemoryStorage.Add(ClientGameScreenBook.BuildInfoWindow, new BuildInfoWindowState());
            inMemoryStorage.Add(ClientGameScreenBook.UpdateInfoWindow, new UpdateInfoWindowState());

            inMemoryStorage.Add(ClientGameScreenBook.BuildCharacterWindow, new BuildCharacterWindowState());
            inMemoryStorage.Add(ClientGameScreenBook.UpdateCharacterWindow, new UpdateCharacterWindowState(new Core.FXBehaviour.FadeToBehaviour()));

            var getDelegation = new GetNetworkGameScreenDelegate(() => inMemoryStorage);
            var setDelegation = new SetNetworkGameScreenDelegate(() => inMemoryStorage);
            var newDelegation = new NewNetworkGameScreenDelegate(() => inMemoryStorage);
            return new ClientGameContext(getDelegation, setDelegation, newDelegation);
        }
    }
}
