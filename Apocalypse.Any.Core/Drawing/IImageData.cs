using Apocalypse.Any.Core.Behaviour;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Drawing
{
    public interface IImageData : IFullPositionHolder
    {
        AlphaBehaviour Alpha { get; set; }
        Color Color { get; set; }
        float LayerDepth { get; set; }
        string SelectedFrame { get; set; }
        string Path { get; set; }
        Vector2 Scale { get; set; }
        float Width { get; }
        float Height { get; }
    }
}