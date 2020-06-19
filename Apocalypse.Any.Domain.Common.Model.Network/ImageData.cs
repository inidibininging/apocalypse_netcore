using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Model;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class ImageData : IImageData, IIdentifiableModel
    {
        public string Id { get; set; }
        public AlphaBehaviour Alpha { get; set; }
        public Color Color { get; set; }
        public float LayerDepth { get; set; }
        public string Path { get; set; }
        public string SelectedFrame { get; set; }
        public MovementBehaviour Position { get; set; }
        public RotationBehaviour Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}