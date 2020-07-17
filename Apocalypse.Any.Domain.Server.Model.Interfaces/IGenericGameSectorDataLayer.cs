using Apocalypse.Any.Domain.Server.Model.Interfaces;
using System.Collections.Concurrent;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    public interface IGenericGameSectorDataLayer
    {
        ConcurrentBag<IGenericTypeDataLayer> Layers { get; set; }
    }
}