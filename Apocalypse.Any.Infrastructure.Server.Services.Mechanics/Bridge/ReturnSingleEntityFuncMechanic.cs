using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Bridge
{
    public class ReturnSingleEntityFuncMechanic<TCharacter> : ISingleCharacterEntityMechanic<TCharacter>
        where TCharacter : CharacterEntity, new()
    {
        public bool Active { get; set; } = true;

        public TCharacter Update(TCharacter singularEntity)
        {
            return singularEntity;
        }
    }
}