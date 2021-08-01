using Apocalypse.Any.Domain.Server.Sector.Model;

namespace Apocalypse.Any.GameServer.Services
{
    public class GameSectorRoutePairService
    {
        public GameSectorRoutePairService()
        {
        }

        public GameSectorRoutePair CreateRoutePair(GameSectorTrespassingDirection trespassingDirection, int sourceSector, int destinationSector)
        {
            return new GameSectorRoutePair()
            {
                Trespassing = trespassingDirection,
                GameSectorTag = sourceSector,
                GameSectorDestinationTag = destinationSector,
            };
        }
    }
}