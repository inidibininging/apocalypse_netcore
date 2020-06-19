using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomAsteroidFactory : CheckWithReflectionFactoryBase<ImageData>
    {
        private string Path { get; set; } = "Image/gamesheetExtended";
        private string IdPrefix { get; set; } = "asteroid";

        public override bool CanUse<TParam>(TParam instance)
        {
            return CanUseByTType<TParam, IGameSectorBoundaries>();
        }

        protected override ImageData UseConverter<TParam>(TParam parameter)
        {
            var sectorBoundaries = parameter as IGameSectorBoundaries;
            var x = Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorX);
            var y = Randomness.Instance.From(sectorBoundaries.MinSectorY, sectorBoundaries.MaxSectorY);
            return new ImageData()
            {
                Alpha = new AlphaBehaviour() { Alpha = 1.00f },
                SelectedFrame = $"{IdPrefix}_{Randomness.Instance.From(0, 7)}_{(Randomness.Instance.From(4, 6))}",
                Color = Color.White,
                Scale = new Vector2((float)(Randomness.Instance.From(0, 200) / 100)),
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