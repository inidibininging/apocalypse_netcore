﻿using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Server.Model;
using States.Core.Infrastructure.Services;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IGameSectorsOwner
    {
        Dictionary<string, IStateMachine<string, IGameSectorLayerService>> GameSectorLayerServices { get; set; }
        IGameSectorLayerServiceStateMachineFactory<GameServerConfiguration> SectorStateMachine { get; set; }
        IList<ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>> SectorsOwnerMechanics { get; set; }
    }
}