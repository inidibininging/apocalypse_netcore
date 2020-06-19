using Apocalypse.Any.Domain.Common.Model;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface IEnumerableCharacterEntityTransformationMechanic<TSourceEntity, TDestinationEntity>
        where TSourceEntity : CharacterEntity, new()
        where TDestinationEntity : CharacterEntity, new()
    {
        IEnumerable<TDestinationEntity> Update(IEnumerable<TSourceEntity> source);
    }
}