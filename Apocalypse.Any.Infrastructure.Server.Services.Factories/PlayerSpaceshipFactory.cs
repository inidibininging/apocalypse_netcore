using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Domain.Server.Model;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories
{
    public class PlayerSpaceshipFactory : CheckWithReflectionFactoryBase<PlayerSpaceship>
    {
        public string IdPrefix { get; set; } = "player";

        public override bool CanUse<TParam>(TParam instance) => typeof(TParam) == typeof(string);
        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(string) };
        }

        protected override PlayerSpaceship UseConverter<TParam>(TParam parameter)
        {
            var loginToken = parameter as string;
            var yFrame = Randomness.Instance.From(0, 4);
            var xFrame = Randomness.Instance.From(0, yFrame == 4 ? 3 : 4);
            
            return new PlayerSpaceship()
            {
                Name = loginToken,
                LoginToken = loginToken,
                Stats = new CharacterSheet()
                {
                    Speed = 5,
                    Attack = 1,
                    Defense = 5,
                    Aura = 5,
                    Strength = 5,
                    Health = 100,
                    Experience = 0,
                },
                CurrentImage = new ImageData()
                {
                    Id = Guid.NewGuid().ToString(),
                    Alpha = new AlphaBehaviour() { Alpha = 1 },
                    Path = "Image/ships",
                    SelectedFrame = $"{IdPrefix}_{xFrame}_{yFrame}",
                    Height = 32,
                    Width = 32,
                    Scale = new Vector2(1.5f, 1.5f),
                    Color = Color.White,
                    Position = new MovementBehaviour() { X = 0, Y = 0 },
                    Rotation = new RotationBehaviour() { Rotation = 180 },
                    LayerDepth = DrawingPlainOrder.Entities
                },
                Tags = new List<string>() { "Players" }
            };
        }
    }
}