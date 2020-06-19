using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core
{
    public interface IGameObject : IGameComponent, IContentHolder, IUpdateableLite
    {
        //Dictionary<string, IGameObject> Objects { get; set;}
    }
}