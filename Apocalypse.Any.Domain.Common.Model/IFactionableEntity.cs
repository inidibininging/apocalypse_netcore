using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Model
{
    /// <summary>
    /// This describes the alliance an entity to n factions
    /// </summary>
    public interface IFactionableEntity
    {
        List<string> Factions { get; set; }
    }
}