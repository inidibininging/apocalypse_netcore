using Apocalypse.Any.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public struct DeltaImageData : IIdentifiableModel
    {
        public string Id { get; set; }
        public float? Alpha { get; set; } // from 0 to 1
        public byte? R { get; set; }
        public byte? G { get; set; }
        public byte? B { get; set; }
        public float? LayerDepth { get; set; }
        public int Path { get; set; }
        public (int frame,int x, int y) SelectedFrame { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }
        public float? Rotation { get; set; }
        public float? ScaleX { get; set; }
        public float? ScaleY { get; set; }

        // public float? Width { get; set; }
        // public float? Height { get; set; }

        public static implicit operator ImageData(DeltaImageData deltaImageData)
        {
            return new ImageData()
            {
                Id = deltaImageData.Id,
                Path = deltaImageData.Path,
                SelectedFrame = deltaImageData.SelectedFrame,
                // Width = deltaImageData.Width.Value,
                // Height = deltaImageData.Height.Value,
                Alpha = new Core.Behaviour.AlphaBehaviour()
                {
                    Alpha = deltaImageData.Alpha.Value
                },
                Color = new Microsoft.Xna.Framework.Color(
                        deltaImageData.R.Value,
                        deltaImageData.G.Value,
                        deltaImageData.B.Value
                    ),
                LayerDepth = deltaImageData.LayerDepth.Value,
                Position = new Core.Behaviour.MovementBehaviour()
                {
                    X = deltaImageData.X.Value,
                    Y = deltaImageData.Y.Value
                },
                Rotation = new Core.Behaviour.RotationBehaviour()
                {
                    Rotation = deltaImageData.Rotation.Value
                },
                Scale = new Microsoft.Xna.Framework.Vector2(deltaImageData.ScaleX.Value, deltaImageData.ScaleY.Value)
            };
        }
    }
}
