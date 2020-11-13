using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomAsteroidFactory : CheckWithReflectionFactoryBase<ImageData>
    {
        private string Path { get; set; } = "Image/gamesheetExtended";
        private string IdPrefix { get; set; } = "asteroid";

        public override bool CanUse<TParam>(TParam instance) => CanUseByTType<TParam, IGameSectorBoundaries>();

        public override List<Type> GetValidParameterTypes() => new List<Type>() { typeof(IGameSectorBoundaries) };

        private static (int frame, int x, int y) GetRandomAsteroidFrame() => (ImagePaths.AsteroidFrame, Randomness.Instance.From(0, 7), Randomness.Instance.From(4, 6));
        protected override ImageData UseConverter<TParam>(TParam parameter)
        {
            var sectorBoundaries = parameter as IGameSectorBoundaries ?? throw new ArgumentNullException(nameof(parameter));
            var x = Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorX);
            var y = Randomness.Instance.From(sectorBoundaries.MinSectorY, sectorBoundaries.MaxSectorY);
            return new ImageData()
            {
                Alpha = new AlphaBehaviour() { Alpha = 1.00f },
                SelectedFrame = GetRandomAsteroidFrame(),
                Color = Color.White,
                Scale = new Vector2(Randomness.Instance.From(0, 200) / 100f),
                Position = new MovementBehaviour()
                {
                    X = x,
                    Y = y
                },
                Rotation = new RotationBehaviour()
                {
                    Rotation = Randomness.Instance.From(0, 360)
                },
                LayerDepth = DrawingPlainOrder.Background
            };
        }
    }
}