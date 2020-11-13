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
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public class RandomEnemySpaceshipFactory : CheckWithReflectionFactoryBase<EnemySpaceship>
    {
        public string IdPrefix { get; set; } = "enemy";

        public override bool CanUse<TParam>(TParam instance) => CanUseByTType<TParam, string>();

        public override List<Type> GetValidParameterTypes() => new List<Type>() { typeof(string) };

        private static CharacterSheetFactory RandomSheet { get; set; } = new CharacterSheetFactory();

        static (int frame, int x, int y) RandomEnemyFrame() => (ImagePaths.EnemyFrame, Randomness.Instance.From(0, 8), Randomness.Instance.From(0, 1));
        
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
                    Path = ImagePaths.gamesheetExtended, //TODO: look for Image/gamesheetExtended when replacing it with an atlas
                    SelectedFrame = RandomEnemyFrame(),
                    Height = 32,
                    Width = 32,
                    Scale = new Vector2(1.5f, 1.5f),
                    Color = Color.White,
                    Position = new MovementBehaviour() { X = 0, Y = 0 },
                    Rotation = new RotationBehaviour() { Rotation = 180 },
                    LayerDepth = DrawingPlainOrder.Entities
                },
                Tags = GetTags
            };
        }

        private static List<string> GetTags => new List<string>() { "Enemies","Generated" };
    }
}