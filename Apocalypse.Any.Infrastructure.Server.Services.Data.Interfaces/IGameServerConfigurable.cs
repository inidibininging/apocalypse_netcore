using Apocalypse.Any.Domain.Server.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IGameServerConfigurable
    {
        GameServerConfiguration Configuration { get; }
    }
}