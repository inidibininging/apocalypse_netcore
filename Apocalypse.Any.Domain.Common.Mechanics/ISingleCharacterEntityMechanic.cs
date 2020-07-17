using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface ISingleCharacterEntityMechanic<TEntity>
        : ISingleMechanic<TEntity, CharacterEntity>
        where TEntity : CharacterEntity, new()
    {
        
        TEntity Update(TEntity singularEntity);
    }
}