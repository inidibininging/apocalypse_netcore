using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces
{
    public interface IEventDispatcher
    {
        void DispatchEvents(GameTime gameTime);
    }
}