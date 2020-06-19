using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories
{
    public class MockItemFactory : CheckWithReflectionFactoryBase<Item>//, IItemFactory
    {
        private string IdPrefix { get; set; } = "minerals";
        private CharacterSheetFactory CharacterSheetFactory { get; set; }
        public SectorRandomPositionFactory RandomSectorPositionGenerator { get; set; }
        private ICharacterNameGenerator<Item> NameGenerator { get; set; }

        public MockItemFactory(
            CharacterSheetFactory characterSheetFactory,
            SectorRandomPositionFactory sectorRandomPositionFactory,
            ICharacterNameGenerator<Item> itemNameGenerator)
        {
            if (characterSheetFactory == null)
                throw new ArgumentNullException(nameof(characterSheetFactory));
            CharacterSheetFactory = characterSheetFactory;

            if (sectorRandomPositionFactory == null)
                throw new ArgumentNullException(nameof(sectorRandomPositionFactory));
            RandomSectorPositionGenerator = sectorRandomPositionFactory;

            if (itemNameGenerator == null)
                throw new ArgumentNullException(nameof(itemNameGenerator));
            NameGenerator = itemNameGenerator;
        }

        //public Item Create(IGameSectorBoundaries sectorBoundaries)
        //{
        //}

        private Item GenerateItem(
            ICharacterSheet characterSheet,
            Vector2 generatedPosition) => new Item()
            {
                Name = "Item", // needs a ItemNameGenerator, based on stats
                Used = false,
                InstantUse = Randomness.Instance.TrueOrFalse(),
                Stats = characterSheet as CharacterSheet, //OUCH !!! Design fail
                Factions = new List<string>() { "Items", "Generated"},
                CurrentImage = new ImageData()
                {
                    Id = $"itm_{Guid.NewGuid().ToString()}",
                    Alpha = new AlphaBehaviour() { Alpha = 1.0f },
                    Path = "Image/gamesheetExtended",
                    SelectedFrame = $"{IdPrefix}_{Randomness.Instance.From(0, 7)}_{Randomness.Instance.From(2, 3)}",
                    Height = 32,
                    Width = 32,
                    Scale = new Vector2(1),
                    Color = new Color
                            (
                                                100,
                                                255,
                                                255
                            ),
                    Position = new MovementBehaviour()
                    {
                        X = generatedPosition.X,
                        Y = generatedPosition.Y
                    },
                    Rotation = new RotationBehaviour() { Rotation = Randomness.Instance.From(0, 360) },
                    
                }
            };

        private Item GenerateExperienceChunkItem(int exp, Vector2 generatedPosition) => new Item()
        {
            Name = "MockItem of Great EXP", // needs a ItemNameGenerator, based on stats
            Used = false,
            InstantUse = true,
            Stats = new CharacterSheet()
            {
                Experience = exp
            },
            CurrentImage = new ImageData()
            {
                Id = $"{Guid.NewGuid().ToString()}",
                Alpha = new AlphaBehaviour() { Alpha = 1.0f },
                Path = "Image/gamesheetExtended",
                SelectedFrame = $"{IdPrefix}_{Randomness.Instance.From(0, 7)}_{Randomness.Instance.From(2, 3)}",
                Height = 32,
                Width = 32,
                Scale = new Vector2(1),
                Color = new Color
                            (
                                                50,//Randomness.Instance.From(100,255),
                                                exp,
                                                exp
                            ),
                Position = new MovementBehaviour()
                {
                    X = generatedPosition.X,
                    Y = generatedPosition.Y
                },
                Rotation = new RotationBehaviour() { Rotation = Randomness.Instance.From(0, 360) }
            }
        };

        public override bool CanUse<TParam>(TParam instance)
        {
            return CanUseByTType<TParam, IGameSectorBoundaries>();
        }

        protected override Item UseConverter<TParam>(TParam parameter)
        {
            var sectorBoundaries = parameter as IGameSectorBoundaries;
            
            var randomSheet = CharacterSheetFactory.GetRandomSheet();
            Console.WriteLine($"random sheet created with {randomSheet.Speed} ag & {randomSheet.Attack} atk");
            var exp = Randomness.Instance.From(1, 255);

            var generatedPosition = Vector2.Zero;
            try
            {
                generatedPosition = RandomSectorPositionGenerator.Create(sectorBoundaries);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            var item = GenerateItem(randomSheet, generatedPosition);
            item.Name = NameGenerator.Generate(item);
            return item;
        }
    }
}