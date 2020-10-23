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
        public int Path { get; set; }
        public (int frame,int x, int y)  SelectedFrame { get; set; }
        public MovementBehaviour Position { get; set; }
        public RotationBehaviour Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public static implicit operator DeltaImageData(ImageData img)
        {
            return new DeltaImageData()
            {
                Id = img.Id,
                LayerDepth = img.LayerDepth,
                Alpha = img.Alpha.Alpha,
                X = img.Position.X,
                Y = img.Position.Y,
                Rotation = img.Rotation.Rotation,
                Width = img.Width,
                Height = img.Height,
                ScaleX = img.Scale.X,
                ScaleY = img.Scale.Y,
                R = img.Color.R,
                G = img.Color.G,
                B = img.Color.B,
                Path = img.Path,
                SelectedFrame = img.SelectedFrame,
            };
        }
    }
}