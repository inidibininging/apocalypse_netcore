using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;

namespace Apocalypse.Any.Domain.Server.Sector.Model
{
    public class GameSectorLoginTokenBag
    {
        public string LoginToken { get; set; }
        public IGameSectorLayerService GameSectorLayerService { get; set; }
    }
}