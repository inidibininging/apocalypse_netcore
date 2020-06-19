using Apocalypse.Any.Core.Drawing;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface IEnumerIEnumerableFullPositionHolderTransformationMechanicableEntityTransformationMechanic<TSourceEntity, TDestinationEntity>
        where TSourceEntity : IFullPositionHolder, new()
        where TDestinationEntity : IFullPositionHolder, new()
    {
        IEnumerable<TDestinationEntity> Update(IEnumerable<TSourceEntity> source);
    }
}