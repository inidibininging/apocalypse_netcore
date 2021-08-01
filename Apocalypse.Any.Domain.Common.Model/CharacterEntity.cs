using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;

namespace Apocalypse.Any.Domain.Common.Model
{
    /// <summary>
    /// An entity with a character sheet, image(will be deprecated)
    /// TODO: decouple any image information from the entity
    /// </summary>
    public class CharacterEntity : ICharacterEntity, ITagableEntity, IIdentifiableModel, IDisplayableByName
    {
        //TODO: make this unique and not bound to name. Name should be named "DisplayName"
        public string Id { get => DisplayName; set => DisplayName = value; }
        public string DisplayName { get; set; }
        public CharacterSheet Stats { get; set; }
        public ImageData CurrentImage { get; set; }
        public List<string> Tags { get; set; }

        public void AddReplaceTag(string tag)
        {
            if (Tags == null)
                return;
            if (Tags.Contains(tag))
                return;
            Tags.Add(tag);
        }
    }
}