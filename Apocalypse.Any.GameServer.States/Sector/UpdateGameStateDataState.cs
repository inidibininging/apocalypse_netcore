using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Transformations;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector.Factories
{
    public class UpdateGameStateDataState : IState<string, IGameSectorLayerService>
    {
        public PlayerSpaceshipUpdateGameStateFactoryRelativeToPlayer RelativeToPlayer { get; set; } = new PlayerSpaceshipUpdateGameStateFactoryRelativeToPlayer();
        public IGenericTypeFactory<GameStateData> PlayerViewFactory { get; }

        public UpdateGameStateDataState(IGenericTypeFactory<GameStateData> playerViewFactory)
        {
            PlayerViewFactory = playerViewFactory ?? throw new ArgumentNullException(nameof(playerViewFactory));
        }
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            //TODO: This can be done in parallel right?
            machine.SharedContext.DataLayer.Players.ToList().ForEach(player =>
            {
                var cache = PlayerViewFactory
                            .Create(new GameSectorLoginTokenBag()
                            {
                                LoginToken = player.LoginToken,
                                GameSectorLayerService = machine.SharedContext
                            });
                //cache = RelativeToPlayer.Create(cache);
                machine.SharedContext.IODataLayer.ForwardServerDataToGame(cache);
            });
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //machine.SharedContext.Messages.ToList().ForEach(msg => Console.WriteLine(msg));
            machine.SharedContext.Messages.Clear();
            //Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}