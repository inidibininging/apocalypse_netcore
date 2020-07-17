using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomPlanetFactory : CheckWithReflectionFactoryBase<ImageData>
    {
        public string IdPrefix { get; set; } = "planetsRandom";

        public override bool CanUse<TParam>(TParam instance) => CanUseByTType<TParam, IGameSectorBoundaries>();
        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(IGameSectorBoundaries) };
        }
        protected override ImageData UseConverter<TParam>(TParam parameter)
        {
            var sectorBoundaries = parameter as IGameSectorBoundaries;
            return new ImageData()
            {
                Id = $"{IdPrefix}{Guid.NewGuid().ToString()}",
                Alpha = new AlphaBehaviour() { Alpha = 1.0f },
                Path = $"Image/{IdPrefix}{Randomness.Instance.From(0, 2)}_edit",
                SelectedFrame = $"{IdPrefix}{Randomness.Instance.From(0, 2)}_{Randomness.Instance.From(0, 7)}_{Randomness.Instance.From(0, 7)}",
                Height = 128,
                Width = 128,
                Scale = new Vector2(Randomness.Instance.From(1, 4)),
                Color = new Color
                            (
                                                Randomness.Instance.From(100, 255),
                                                Randomness.Instance.From(100, 255),
                                                Randomness.Instance.From(100, 255)
                            ),
                Position = new MovementBehaviour()
                {
                    X = Randomness.Instance.RollTheDice(5) ? Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorX) : Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorX),
                    Y = Randomness.Instance.RollTheDice(15) ? Randomness.Instance.From(sectorBoundaries.MinSectorY, sectorBoundaries.MaxSectorY) : Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorY)
                },
                Rotation = new RotationBehaviour() { Rotation = Randomness.Instance.From(0, 360) },
                LayerDepth = DrawingPlainOrder.Background + DrawingPlainOrder.MicroPlainStep
            };
        }
    }
}