using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Domain.Server.Sector.Model;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    internal class GameSectorTrespassingRouteFactory : CheckWithReflectionFactoryBase<IDictionary<GameSectorRoute, GameSectorRoute>>
    {
        public override bool CanUse<TParam>(TParam instance)
        {
            return CanUseByTType<TParam, IEnumerable<IGameSectorData>>();
        }

        protected override IDictionary<GameSectorRoute, GameSectorRoute> UseConverter<TParam>(TParam parameter)
        {
            return null;
            //return from gameSectorData in (parameter as IEnumerable<IGameSectorData)
            //       select new GameSectorRoutePair()
            //       {
            //           GameSectorTag = gameSectorData.Tag,
            //           GameSectorDestinationTag = gameSectorData.
            //       }
        }
    }
}