using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Apocalypse.Any.Infrastructure.Server.Adapters.Redis
{
    public interface IGameSectorOwnerRedisMechanic : ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>
    {
        string RedisHost { get; set; }
        int RedisPort { get; set; }
        bool Executing { get; }
        
    }
}
