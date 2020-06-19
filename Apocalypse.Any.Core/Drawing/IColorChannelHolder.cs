using Apocalypse.Any.Core.Behaviour;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// This class describes the full color channel of an object.
    /// </summary>
    public interface IColorChannelHolder
    {
        Color Color { get; set; }
        AlphaBehaviour Alpha { get; set; }
    }
}