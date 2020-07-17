using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Model
{
    /// <summary>
    /// This describes the tags an entity can have. 
    /// This can be used for anything
    /// Internally, this is used:
    /// - with the scripting language for assigning "tags" to enemies
    /// - dividing entities into factions
    /// - events
    /// </summary>
    public interface IFactionableEntity
    {
        List<string> Tags { get; set; }
    }
}