using Apocalypse.Any.Domain.Common.Model.RPG;

namespace Apocalypse.Any.Domain.Common.Model
{
    public interface ICharacterEntity : IEntityWithImage //TODO: break up Ientity with image to single interfaces
    {
        CharacterSheet Stats { get; set; }
    }
}