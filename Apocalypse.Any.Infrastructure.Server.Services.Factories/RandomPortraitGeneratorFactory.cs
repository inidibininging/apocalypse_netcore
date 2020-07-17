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

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomPortraitGeneratorFactory : CheckWithReflectionFactoryBase<ImageData>
    {
        private string IdPrefix { get; set; } = "faces";

        public override bool CanUse<TParam>(TParam instance)
        {
            return CanUseByTType<TParam, MovementBehaviour>();
        }

        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type> { typeof(MovementBehaviour) };
        }

        protected override ImageData UseConverter<TParam>(TParam parameter)
        {
            
            var yFrame = Randomness.Instance.From(0, 3);
            var xFrame = Randomness.Instance.From(0, yFrame == 3 ? 3 : 4);
            var size = 5;
            return new ImageData()
            {
                Id = $"itm_{Guid.NewGuid().ToString()}",
                Alpha = new AlphaBehaviour() { Alpha = 1.0f },
                Path = "Image/faces",
                SelectedFrame = $"{IdPrefix}_{xFrame}_{yFrame}",
                Height = 32 * size,
                Width = 32 * size,
                Scale = new Vector2(size),
                Color = new Color
                            (
                                                255,
                                                128,
                                                255
                            ),
                Position = new MovementBehaviour()
                {
                    X = (parameter as MovementBehaviour).X,
                    Y = (parameter as MovementBehaviour).Y
                },
                Rotation = new RotationBehaviour() { Rotation = 0 },//Randomness.Instance.From(0, 360) },
                LayerDepth = DrawingPlainOrder.UI
            };
        }
    }
}
