using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomTilesetPartFactory : CheckWithReflectionFactoryBase<ImageData>
    {
        private int Path { get; }
        public List<(int frame, int x, int y)> Frames { get; set; }

        public RandomTilesetPartFactory(int path,                                        
                                        List<(int frame, int x, int y)> frames)
        {            
            Path = path;
            Frames = frames;
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
                Id = Guid.NewGuid().ToString(),
                Alpha = new AlphaBehaviour() { Alpha = 1.00f },
                Path = this.Path,
                //SelectedFrame = $"{IdPrefix}_{Randomness.Instance.From(StartX, EndX)}_{(Randomness.Instance.From(StartY, EndY))}",
                SelectedFrame = Frames.ElementAt(Randomness.Instance.From(0, Frames.Count - 1)),
                Color = Color.White,
                Scale = new Vector2(32),
                Position = new MovementBehaviour()
                {
                    X = position.X,
                    Y = position.Y
                },
                Rotation = new RotationBehaviour(),
                Width = 1024,
                Height = 1024,
                LayerDepth = DrawingPlainOrder.Entities - DrawingPlainOrder.PlainStep
            };
        }

        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(MovementBehaviour) };
        }
    }
}