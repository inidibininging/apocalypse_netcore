using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface IEnumerableCharacterEntityMechanic<TEntity>
    : IEnumerableMechanic<TEntity, CharacterEntity>
    where TEntity : CharacterEntity, new()
    {
    }
}