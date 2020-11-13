using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomPortraitGeneratorFactory : CheckWithReflectionFactoryBase<ImageData>
    {
        private int IdPrefix { get; set; } = ImagePaths.faces;

        public override bool CanUse<TParam>(TParam instance) => CanUseByTType<TParam, MovementBehaviour>();

        public override List<Type> GetValidParameterTypes() => new List<Type> { typeof(MovementBehaviour) };

        protected override ImageData UseConverter<TParam>(TParam parameter)
        {
            var portraitPosition = parameter as MovementBehaviour ?? throw new ArgumentNullException(nameof(parameter));
            var yFrame = Randomness.Instance.From(0, 3);
            var xFrame = Randomness.Instance.From(0, yFrame == 3 ? 3 : 4);
            var size = 5;
            return new ImageData()
            {
                Id = $"itm_{Guid.NewGuid().ToString()}",
                Alpha = new AlphaBehaviour() { Alpha = 1.0f },
                Path = IdPrefix,
                SelectedFrame = (ImagePaths.FaceFrame, xFrame, yFrame),
                Height = 32 * size,
                Width = 32 * size,
                Scale = new Vector2(size),
                Color = GetPortraitColor<TParam>(),
                Position = new MovementBehaviour()
                {
                    X = portraitPosition.X,
                    Y = portraitPosition.Y
                },
                Rotation = new RotationBehaviour() { Rotation = 0 },
                LayerDepth = DrawingPlainOrder.UI
            };
        }

        private static Color GetPortraitColor<TParam>()
        {
            return new Color
            (
                255,
                128,
                255
            );
        }
    }
}
