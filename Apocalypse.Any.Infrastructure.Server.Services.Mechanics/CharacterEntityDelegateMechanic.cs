using System;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class CharacterEntityDelegateMechanic<TCharacterEntity>
        : ISingleCharacterEntityMechanic<TCharacterEntity>
        where TCharacterEntity : CharacterEntity, new()
    {
        public bool Active { get; set; } = true;
        Func<TCharacterEntity,TCharacterEntity> CharacterEntityDelegate { get; set; }
        public CharacterEntityDelegateMechanic(Func<TCharacterEntity,TCharacterEntity> characterEntityDelegate)
        {
            CharacterEntityDelegate = characterEntityDelegate ?? throw new ArgumentNullException(nameof(characterEntityDelegate));
        }
        public TCharacterEntity Update(TCharacterEntity singularEntity)
        {
            return CharacterEntityDelegate?.Invoke(singularEntity);
        }
    }
}
