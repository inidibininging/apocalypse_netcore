using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Apocalypse.Any.Core.Utilities
{
    public static class TextureExtensions
    {
        public static Vector2 GetWidthAndHeight(this string imgPath, GraphicsDevice device)
        {
            int width, height = 0;
            using (var stream = new FileStream(imgPath, FileMode.Open))
            using (Texture2D tex = Texture2D.FromStream(device, stream))
            {
                width = tex.Width;
                height = tex.Height;
            }
            return new Vector2(width, height);
        }
    }
}