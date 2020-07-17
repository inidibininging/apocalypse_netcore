using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class SectorRandomPositionFactory : CheckWithReflectionFactoryBase<Vector2>
    {
        public override bool CanUse<TParam>(TParam instance)
        {
            return CanUseByTType<TParam, IGameSectorBoundaries>();
        }
        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(IGameSectorBoundaries) };
        }
        protected override Vector2 UseConverter<TParam>(TParam parameter)
        {            
            var sectorBoundaries = parameter as IGameSectorBoundaries;

            //TODO: ITS A BUUUUG .... fucker
            if(sectorBoundaries.MinSectorX > sectorBoundaries.MaxSectorX)
            {
                var minX = sectorBoundaries.MaxSectorX;
                var maxX = sectorBoundaries.MinSectorX;
                sectorBoundaries.MinSectorX = minX;
                sectorBoundaries.MaxSectorX = maxX;
            }
            if(sectorBoundaries.MinSectorY > sectorBoundaries.MaxSectorY)
            {
                var minY =  sectorBoundaries.MaxSectorY;
                var maxY =  sectorBoundaries.MinSectorY;
                sectorBoundaries.MinSectorY = minY;
                sectorBoundaries.MaxSectorY = maxY;
            }

            return new Vector2(Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorX),
                               Randomness.Instance.From(sectorBoundaries.MinSectorY, sectorBoundaries.MaxSectorY));

        }
    }
}