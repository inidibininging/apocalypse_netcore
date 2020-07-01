using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomTilesetPartFactory : CheckWithReflectionFactoryBase<ImageData>
    {
        private string Path { get; }
        private string IdPrefix { get; }
        private int StartX { get; }
        private int EndX { get; }
        private int StartY { get; }
        private int EndY { get; }

        public RandomTilesetPartFactory(string path,
                                        string idPrefix,
                                        int startX, int endX,
                                        int startY, int endY)
        {
            IdPrefix = idPrefix;
            Path = path;
            StartX = startX;
            EndX = endX;
            StartY = startY;
            EndY = endY;
        }
        public override bool CanUse<TParam>(TParam instance)
        {
            return CanUseByTType<TParam, MovementBehaviour>();
        }

        protected override ImageData UseConverter<TParam>(TParam parameter)
        {
            var position = parameter as MovementBehaviour;
            return new ImageData()
            {
                Alpha = new AlphaBehaviour() { Alpha = 1.00f },
                SelectedFrame = $"{IdPrefix}_{Randomness.Instance.From(StartX, EndX)}_{(Randomness.Instance.From(StartY, EndY))}",
                Color = Color.White,
                Scale = new Vector2(1),
                Position = new MovementBehaviour()
                {
                    X = position.X,
                    Y = position.Y
                },
                Rotation = new RotationBehaviour(),
                LayerDepth = DrawingPlainOrder.EntitiesFX + DrawingPlainOrder.PlainStep
            };
        }
    }
}