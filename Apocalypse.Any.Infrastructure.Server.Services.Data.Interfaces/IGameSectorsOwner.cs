using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using States.Core.Infrastructure.Services;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    ///<summary>
    /// Provides an interface to a sector owner.For now, a sector owner is for example any instance of IWolrdGame
    /// The main rease for this interface is the reduction of an interface to its sectors and sector wide mechanics 
    /// see ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>
    ///</summary>
    public interface IGameSectorsOwner : IGameServerConfigurable
    {
        Dictionary<int, IStateMachine<string, IGameSectorLayerService>> GameSectorLayerServices { get; set; }
        IGameSectorLayerServiceStateMachineFactory<GameServerConfiguration> SectorStateMachine { get; set; }
        IList<ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>> SectorsOwnerMechanics { get; set; }
    }
}