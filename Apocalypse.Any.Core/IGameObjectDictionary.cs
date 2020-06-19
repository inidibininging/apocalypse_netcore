using System.Collections.Generic;

namespace Apocalypse.Any.Core
{
    public interface IGameObjectDictionary : IGameObject, IDictionary<string, IGameObject>
    {
        T As<T>(string name) where T : IGameObject;
    }
}