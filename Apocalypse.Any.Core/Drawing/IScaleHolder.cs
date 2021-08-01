

using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Drawing
{
    public interface IScaleHolder
    {
        /// <summary>
        /// Represents the scale factor of a rectangular image
        /// </summary>
        Vector2 Scale { get; set; }
    }
}