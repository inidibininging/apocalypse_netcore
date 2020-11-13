using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Collections.Generic;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;

namespace Apocalypse.Any.GameServer.GameInstance
{
    public interface IWorldGame
    {
        GameServerConfiguration Configuration { get; set; }
        Dictionary<string, IStateMachine<string, IGameSectorLayerService>> GameSectorLayerServices { get; set; }
        PlayerSpaceshipFactory PlayerFactory { get; set; }
        IList<ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>> SectorsOwnerMechanics { get; set; }
        IGameSectorLayerServiceStateMachineFactory<GameServerConfiguration> SectorStateMachine { get; set; }

        void BroadcastMessage(string message);
        bool ForwardClientDataToGame(GameStateUpdateData updateData);
        bool ForwardServerDataToGame(GameStateData gameStateData);
        GameStateData GetGameStateByLoginToken(string loginToken);
        IGameSectorLayerService GetSector(string sectorIdentifier);
        GameStateData RegisterGameStateData(string loginToken);
        void Update(GameTime gameTime);
    }
}