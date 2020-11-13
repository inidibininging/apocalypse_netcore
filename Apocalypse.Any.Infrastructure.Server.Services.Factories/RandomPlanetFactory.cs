using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomPlanetFactory : CheckWithReflectionFactoryBase<ImageData>
    {
        public string IdPrefix { get; set; } = "planetsRandom";

        public override bool CanUse<TParam>(TParam instance) => CanUseByTType<TParam, IGameSectorBoundaries>();
        public override List<Type> GetValidParameterTypes() => new List<Type>() { typeof(IGameSectorBoundaries) };

        private (int frame, int x, int y) RandomPlanetFrame()
        {
            var randomPlanetId = Randomness.Instance.From(0, 2);
            switch (randomPlanetId)
            {
                case 0:
                    randomPlanetId = ImagePaths.RandomPlanetFrame0;
                    break;
                case 1:
                    randomPlanetId = ImagePaths.RandomPlanetFrame1;
                    break;
                case 2:
                    randomPlanetId = ImagePaths.RandomPlanetFrame2;
                    break;
                default:
                    randomPlanetId = ImagePaths.RandomPlanetFrame0;
                    break;
            }

            return (randomPlanetId, Randomness.Instance.From(0, 7), Randomness.Instance.From(0, 7));

        }
        protected override ImageData UseConverter<TParam>(TParam parameter)
        {
            var sectorBoundaries = parameter as IGameSectorBoundaries;
            return new ImageData()
            {
                Id = $"{IdPrefix}{Guid.NewGuid().ToString()}",
                Alpha = new AlphaBehaviour() { Alpha = 1.0f },
                Path = Randomness.Instance.From(ImagePaths.planetsRandom0_edit, ImagePaths.planetsRandom2_edit),
                SelectedFrame = RandomPlanetFrame(),
                Height = 128,
                Width = 128,
                Scale = new Vector2(Randomness.Instance.From(1, 4)),
                Color = GetRandomColor<TParam>(),
                Position = new MovementBehaviour()
                {
                    X = Randomness.Instance.RollTheDice(5) ? Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorX) : Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorX),
                    Y = Randomness.Instance.RollTheDice(15) ? Randomness.Instance.From(sectorBoundaries.MinSectorY, sectorBoundaries.MaxSectorY) : Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorY)
                },
                Rotation = new RotationBehaviour() { Rotation = Randomness.Instance.From(0, 360) },
                LayerDepth = DrawingPlainOrder.Background + DrawingPlainOrder.MicroPlainStep
            };
        }

        private static Color GetRandomColor<TParam>()
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