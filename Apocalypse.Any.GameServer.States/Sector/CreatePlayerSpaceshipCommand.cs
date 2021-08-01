using Apocalypse.Any.Core;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class CreatePlayerSpaceshipCommand : ICommand<(IGameSectorLayerService GameSector,
                                                          IUserLoginService LoginService)>,
        IState<string, IGameSectorLayerService>
    {
        //private string LoginToken { get; set; }

        public bool CanExecute((IGameSectorLayerService GameSector, IUserLoginService LoginService) parameters)
        {
            if (parameters.GameSector.Factories.PlayerFactory == null)
            {
                parameters.GameSector.Messages.Add("player spaceship cannot be created. player factory is not available");
                return false;
            }

            if (!parameters.Item1.Factories.PlayerFactory.ContainsKey(nameof(PlayerSpaceshipFactory)))
                return false;

            return true;
        }

        //public CreatePlayerSpaceshipCommand(string loginToken)
        //{
        //    if (string.IsNullOrWhiteSpace(loginToken))
        //        throw new ArgumentNullException(nameof(CreatePlayerSpaceshipCommand));
        //    LoginToken = loginToken;
        //}

        public void Execute((IGameSectorLayerService GameSector,
                             IUserLoginService LoginService) parameters)
        {
            if (!CanExecute(parameters))
                return;

            var newPlayer = parameters
                            .GameSector
                            .Factories
                            .PlayerFactory[nameof(PlayerSpaceshipFactory)]
                            .Create(parameters.LoginService);

            if (newPlayer == null)
                return;
            newPlayer.CurrentImage.Position.X = parameters.GameSector.SectorBoundaries.MaxSectorX / 2;
            newPlayer.CurrentImage.Position.Y = parameters.GameSector.SectorBoundaries.MaxSectorY / 2;

            parameters.GameSector.DataLayer.Players.Add(newPlayer);
            parameters.GameSector.Messages.Add($"player spaceship created.{newPlayer.LoginToken}");
        }

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            Execute((machine.SharedContext, null));
        }
    }
}