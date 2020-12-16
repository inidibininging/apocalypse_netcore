using System;
using System.Collections.Generic;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Domain.Server.Model;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
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
                DisplayName = loginToken,
                LoginToken = loginToken,
                Stats = GetCharacterSheet<TParam>(),
                CurrentImage = CreatePlayerSpaceShipImage<TParam>(xFrame, yFrame),
                Tags = GetPlayerTags<TParam>()
            };
        }

        private static CharacterSheet GetCharacterSheet<TParam>()
        {
            return new CharacterSheet()
            {
                Speed = 5,
                Attack = 1,
                Defense = 5,
                Aura = 5,
                Strength = 5,
                Health = 1000,
                Experience = 0,
            };
        }

        private static List<string> GetPlayerTags<TParam>()
        {
            return new List<string>() { "Players" };
        }

        private static ImageData CreatePlayerSpaceShipImage<TParam>(int xFrame, int yFrame)
        {
            return new ImageData()
            {
                Id = Guid.NewGuid().ToString(),
                Alpha = new AlphaBehaviour() { Alpha = 1 },
                Path = ImagePaths.ships,
                SelectedFrame = (ImagePaths.PlayerFrame, xFrame, yFrame),
                Height = 32*2,
                Width = 32*2,
                Scale = new Vector2(2f, 2f),
                Color = Color.DeepPink,
                Position = new MovementBehaviour() { X = 0, Y = 0 },
                Rotation = new RotationBehaviour() { Rotation = 180 },
                LayerDepth = DrawingPlainOrder.Entities
            };
        }
    }
}