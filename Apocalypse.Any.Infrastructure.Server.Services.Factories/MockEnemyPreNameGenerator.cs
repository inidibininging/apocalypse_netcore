using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories
{
    public class MockEnemyPreNameGenerator : ICharacterNameGenerator<EnemySpaceship>
    {
        private List<string> _attack = new List<string>()
        {
            "Urchin",
            "Eyes",
            "Dreadnought",
            "Brutal",
            "Strong",
            "Aggressive",
            "Gamma",
            "Rad",
            "Hitman",
            "Raider",
            "Scavenger",
            "Deletionist",
            "Clean R",
            "Saboteur",
            "Killer",
            "Savage",
            "Intruder"
        };

        public List<string> Attack
        {
            get
            {
                return _attack;
            }
        }

        private List<string> _strength = new List<string>()
        {
            "Mighty",
            "Shielded",
            "Doomed",
            "Yamato",
            "Bold",
            "Steel armored",
            "Fat",
            "Elefant",
            "Millenium",
            "Alpha"
        };

        public List<string> Strength
        {
            get
            {
                return _strength;
            }
        }

        //TODO: Dont use the duplicate
        public List<string> Health
        {
            get
            {
                return _strength;
            }
        }
        public List<string> Defense
        {
            get
            {
                return _strength;
            }
        }

        private List<string> _speed = new List<string>()
        {
            "Light",
            "Fox",
            "Shinano",
            "Rabbit",
            "Agile",
            "Vanguard",
            "Cheetah",
            "Quantum",
            "Beta",
            "Feather",
            "Turbo",
            "Flinker",
            "Z-Class"            
        };

        public List<string> Speed
        {
            get
            {
                return _speed;
            }
        }

        private List<string> _technology = new List<string>()
        {
            "Military",
            "Eyes",
            "Vanguard",
            "Hawking",
            "Turing",
            "DeGrase",
            "Ada",
            "Naburimannu",
            "Su Song",
            "Copernica",
            "New Massachusets"
        };

        public List<string> Technology
        {
            get
            {
                return _technology;
            }
        }

        private List<string> _aura = new List<string>()
        {
            "Panoptical",
            "Chaos",
            "U9157",
            "Unknown",
            "Fraunhofer",
            "Old One",
            "Eye Z",
            "Prophet",
            "Monitor",
            "Indi Go",
            "Maelstrom Class",
            "X-2030"
        };

        public List<string> Aura
        {
            get
            {
                return _aura;
            }
        }

        private List<string> _experience = new List<string>()
        {
            "Knowledgeable",
            "Wise",
            "Novice",
            "Wisdom"
        };

        public List<string> Experience
        {
            get
            {
                return _experience;
            }
        }

        public MockEnemyPreNameGenerator()
        {
        }

        public string Generate(EnemySpaceship entity)
        {
            //TODO: The end result should not be  a string, but rather a string decorator
            if (entity == null)
                Console.WriteLine($"no enemy in {nameof(MockEnemyPreNameGenerator)}");

            var props = typeof(MockEnemyPreNameGenerator).GetProperties();
            // var propSuffix = "Pre";

            //Get the property with the max. stat
            var selectedStat =
            entity
            .Stats
            .GetType()
            .GetProperties()
            .ToList()
            .Where(prop => prop.Name != nameof(Health) && prop.Name != nameof(Defense))
            .Select(prop =>
            {
                var statVal = 0;
                try
                {                    
                    statVal = (int)prop.GetValue(entity.Stats);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ups:{ex.Message}");
                }
                return new System.Tuple<string, int>
                (
                    prop.Name,
                    statVal
                );
            })
            .OrderByDescending(t => t.Item2)
            .First();
            Console.WriteLine($"{nameof(MockEnemyPreNameGenerator)} found a prop");
            Console.WriteLine($"{selectedStat.Item1} {selectedStat.Item2}");
            var foundProp = props.Where(prop => prop.Name == selectedStat.Item1);
            if (foundProp == null || !foundProp.Any())
            {
                Console.WriteLine("no prop found");
                return entity.Name;
            }
            else
            {
                var foundListMaybeNull = foundProp.FirstOrDefault();

                var foundList = ((List<string>)foundListMaybeNull.GetValue(this));

                return $"{foundList.ElementAt(Randomness.Instance.From(0, foundList.Count)).Replace(" ","0")}{Randomness.Instance.From(2000,90000)}";
            }
        }
    }
}