using System.Collections.Generic;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;

namespace Apocalypse.Any.Domain.Common.Model
{
    /// <summary>
    /// An entity with a character sheet, image(will be deprecated) and icon image(will be also deprecated)
    /// TODO: decouple any image information from the entity
    /// </summary>
    public class CharacterEntity : ICharacterEntity, IFactionableEntity
    {
        public string Name { get; set; }
        public CharacterSheet Stats { get; set; }
        public ImageData CurrentImage { get; set; }
        public ImageData IconImage { get; set; } //TODO: decouple this
        public List<string> Factions { get; set; }
    }
}