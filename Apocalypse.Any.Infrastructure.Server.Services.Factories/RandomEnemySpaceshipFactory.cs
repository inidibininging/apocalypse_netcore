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

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomEnemySpaceshipFactory : CheckWithReflectionFactoryBase<EnemySpaceship>
    {
        public string IdPrefix { get; set; } = "enemy";

        public override bool CanUse<TParam>(TParam instance) => CanUseByTType<TParam, string>();

        public override List<Type> GetValidParameterTypes()
        {
            return new List<Type>() { typeof(string) };
        }

        private static CharacterSheetFactory RandomSheet {get;set;} = new CharacterSheetFactory();
        protected override EnemySpaceship UseConverter<TParam>(TParam parameter)
        {
            var enemyName = parameter as string;
            var sheet = RandomSheet.GetRandomSheet();
            return new EnemySpaceship()
            {
                DisplayName = enemyName,
                Stats = new CharacterSheet()
                {
                    Speed = sheet.Speed,
                    Attack = sheet.Attack,
                    Defense = sheet.Defense,
                    Aura = sheet.Aura,
                    Strength = sheet.Strength,
                    Health = 200
                },
                CurrentImage = new ImageData()
                {
                    Id = Guid.NewGuid().ToString(),
                    Alpha = new AlphaBehaviour() { Alpha = 1 },
                    Path = "Image/gamesheetExtended", //TODO: look for Image/gamesheetExtended when replacing it with an atlas
                    SelectedFrame = $"{IdPrefix}_{Randomness.Instance.From(0, 8)}_{Randomness.Instance.From(0, 1)}",
                    Height = 32,
                    Width = 32,
                    Scale = new Vector2(1.5f, 1.5f),
                    Color = Color.White,
                    Position = new MovementBehaviour() { X = 0, Y = 0 },
                    Rotation = new RotationBehaviour() { Rotation = 180 },
                    LayerDepth = DrawingPlainOrder.Entities
                },
                Tags = new List<string>() { "Enemies","Generated" }
            };
        }
    }
}