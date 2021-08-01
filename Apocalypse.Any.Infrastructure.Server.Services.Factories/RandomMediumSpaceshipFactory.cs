using System;
using System.Collections.Generic;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomMediumSpaceshipFactory : CheckWithReflectionFactoryBase<ImageData>
    {
        private const int IdPrefix = ImagePaths.mediumShips_edit;
        public override bool CanUse<TParam>(TParam instance)=> CanUseByTType<TParam,IGameSectorBoundaries>();
        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(IGameSectorBoundaries) };
        }
        private int MaxR { get; set; }
        private int MaxG { get; set; }
        private int MaxB { get; set; }
        (int frame, int x, int y) RandomSpaceshipFrame() => (ImagePaths.MediumShipFrame, Randomness.Instance.From(0, 4), Randomness.Instance.From(0, 4));
        protected override ImageData UseConverter<TParam>(TParam parameter)
        {
            var sectorBoundaries = parameter as IGameSectorBoundaries ?? throw new ArgumentNullException(nameof(parameter));

            var scale = new Vector2(Randomness.Instance.From(1, 15));
            var color = GerRandomColor<TParam>();

            ModifyColorBasedOnScale<TParam>(color, scale);

            return new ImageData()
            {
                Id = $"{IdPrefix}{Guid.NewGuid()}",
                Alpha = new AlphaBehaviour() { Alpha = 1.0f },
                Path = ImagePaths.mediumShips_edit,
                SelectedFrame = RandomSpaceshipFrame(),
                Height = 128,
                Width = 128,
                Scale = scale,
                Color = color,
                Position = new MovementBehaviour()
                {
                    X = Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorX),
                    Y = Randomness.Instance.From(sectorBoundaries.MinSectorY, sectorBoundaries.MaxSectorY)
                },
                Rotation = new RotationBehaviour() { Rotation = Randomness.Instance.From(0, 360) },
                LayerDepth = DrawingPlainOrder.Background + (DrawingPlainOrder.MicroPlainStep * 2)
            };
        }

        private void ModifyColorBasedOnScale<TParam>(Color color, Vector2 scale)
        {
            if (MaxR < color.R)
                MaxR = color.R;
            if (MaxG < color.G)
                MaxG = color.G;
            if (MaxB < color.B)
                MaxB = color.B;

            if (scale.X > 1 && scale.X < 3)
            {
                color.R -= 155;
                color.G -= 155;
                color.B += 25;
            }

            if (scale.X > 3 && scale.X < 5)
            {
                color.R -= 105;
                color.G -= 105;
                color.B += 55;
            }

            if (scale.X > 5 && scale.X < 6)
            {
                color.R -= 75;
                color.G -= 75;
                color.B += 75;
            }
        }

        private static Color GerRandomColor<TParam>()
        {
            return new Color
            (
                Randomness.Instance.From(100, 255),
                Randomness.Instance.From(100, 255),
                Randomness.Instance.From(100, 255)
            );
        }
    }
}