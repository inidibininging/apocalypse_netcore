using System;
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
        private const string IdPrefix = "mediumShips_edit";
        public override bool CanUse<TParam>(TParam instance)=> CanUseByTType<TParam,IGameSectorBoundaries>();
        private int MaxR {get;set;}
        private int MaxG {get;set;}
        private int MaxB {get;set;}
        protected override ImageData UseConverter<TParam>(TParam parameter)
        {
            var sectorBoundaries = parameter as IGameSectorBoundaries;

            var scale = new Vector2(Randomness.Instance.From(1, 15));
            var color = new Color
                            (
                                                Randomness.Instance.From(100, 255),
                                                Randomness.Instance.From(100, 255),
                                                Randomness.Instance.From(100, 255)
                            );

            if(MaxR < color.R)
                MaxR = color.R;
            if(MaxG < color.G)
                MaxG = color.G;
            if(MaxB < color.B)
                MaxB = color.B;
            
            if(scale.X > 1 && scale.X < 3)
            {
                color.R -= 155;
                color.G -= 155;
                color.B += 25;
            }
            if(scale.X > 3 && scale.X < 5)
            {
                color.R -= 105;
                color.G -= 105;
                 color.B += 55;
            }   
            if(scale.X > 5 && scale.X < 6)
            {
                color.R -= 75;
                color.G -= 75;
                color.B += 75;
            } 
            
            return new ImageData()
            {
                Id = $"{IdPrefix}{Guid.NewGuid()}",
                Alpha = new AlphaBehaviour() { Alpha = 1.0f },
                Path = $"Image/{IdPrefix}",
                SelectedFrame = $"{IdPrefix}_{Randomness.Instance.From(0, 4)}_{Randomness.Instance.From(0, 4)}",
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
                LayerDepth = DrawingPlainOrder.Background + (DrawingPlainOrder.MicroPlainStep*2)
            };
        }
    }
}