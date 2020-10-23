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
    public class RandomFogFactory : CheckWithReflectionFactoryBase<ImageData>
    {
        private const int IdPrefix = ImagePaths.fog_edit;
        public override bool CanUse<TParam>(TParam instance)=> CanUseByTType<TParam,IGameSectorBoundaries>();
        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(IGameSectorBoundaries) };
        }
        private int MaxR {get;set;}
        private int MaxG {get;set;}
        private int MaxB {get;set;}
        (int frame, int x, int y) RandomFogFrame() => (ImagePaths.FogFrame, Randomness.Instance.From(0, 2), Randomness.Instance.From(0, 3));
        protected override ImageData UseConverter<TParam>(TParam parameter)
        {
            var sectorBoundaries = parameter as IGameSectorBoundaries;

            var scale = new Vector2(Randomness.Instance.From(1, 8));
            return new ImageData()
            {
                Id = $"{IdPrefix}_{Guid.NewGuid()}",
                Alpha = new AlphaBehaviour() { Alpha = 0.10f },
                Path = IdPrefix,
                SelectedFrame = RandomFogFrame(),
                Height = 512,
                Width = 512,
                Scale = scale,
                Color = 
                    Randomness.Instance.RollTheDice(25) ? Color.DarkViolet :
                    Randomness.Instance.RollTheDice(25) ? Color.Pink :
                    Randomness.Instance.RollTheDice(25) ? Color.LightSkyBlue :
                    Color.WhiteSmoke,
                Position = new MovementBehaviour()
                {
                    X = Randomness.Instance.From(sectorBoundaries.MinSectorX, sectorBoundaries.MaxSectorX),
                    Y = Randomness.Instance.From(sectorBoundaries.MinSectorY, sectorBoundaries.MaxSectorY)
                },
                Rotation = new RotationBehaviour() { Rotation = Randomness.Instance.From(0, 360) },
                LayerDepth = Randomness.Instance.RollTheDice(25) ? DrawingPlainOrder.Background+DrawingPlainOrder.MicroPlainStep : DrawingPlainOrder.Entities+(DrawingPlainOrder.MicroPlainStep*2)
            };
        }
    }
}