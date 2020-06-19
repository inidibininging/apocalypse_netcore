using Microsoft.Xna.Framework.Content;

namespace Apocalypse.Any.Core
{
    public interface IContentHolder
    {
        void LoadContent(ContentManager manager);

        void UnloadContent();
    }
}