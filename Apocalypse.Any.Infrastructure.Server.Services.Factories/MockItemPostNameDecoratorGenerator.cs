using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    /// <summary>
    /// Generates the item name, based on the stats
    /// </summary>
    public class MockItemPostNameDecoratorGenerator : ICharacterNameGenerator<Item>
    {
        private List<string> _attackPost = new List<string>()
        {
            "Razors",
            "Blades",
            "Full Damage",
            "Aggression",
            "Bloodlust",
            "Brutality",
            "Berserk",
            "Evil"
        };

        public List<string> AttackPost
        {
            get
            {
                return _attackPost;
            }
        }

        private List<string> _strengthPost = new List<string>()
        {
            "Might",
            "Metal",
            "Strength",
            "the Bear",
            "Steel",
            "Courage",
            "Heart",
            "Knight"
        };

        public List<string> StrengthPost
        {
            get
            {
                return _strengthPost;
            }
        }

        private List<string> _speedPost = new List<string>()
        {
            "Swift",
            "Fox",
            "Lightning",
            "Quickness",
            "Agility",
            "Cat",
            "Cheetah"
        };

        public List<string> SpeedPost
        {
            get
            {
                return _speedPost;
            }
        }

        private List<string> _technologyPost = new List<string>()
        {
            "Future",
            "Tomorrow",
            "Einstein",
            "Advancement",
            "Science",
            "Wisdom"
        };

        public List<string> TechnologyPost
        {
            get
            {
                return _technologyPost;
            }
        }

        private List<string> _auraPost = new List<string>()
        {
            "Beyond",
            "Chaos",
            "K",
            "Unknown",
            "Ruins",
            "Old One",
            "Eyes",
            "Prophecy"
        };

        public List<string> AuraPost
        {
            get
            {
                return _auraPost;
            }
        }

        private List<string> _experiencePost = new List<string>()
        {
            "Knowledge",
            "the Apprentice",
            "Novice",
            "Wisdom"
        };

        public List<string> ExperiencePost
        {
            get
            {
                return _experiencePost;
            }
        }

        public MockItemPostNameDecoratorGenerator()
        {
        }

        public string Generate(Item item)
        {
            //TODO: The end result should not be  a string, but rather a string decorator
            if (item == null)
                Console.WriteLine("no item");

            var props = this.GetType().GetProperties();
            var propSuffix = "Post";

            //Get the property with the max. stat
            var selectedStat =
            item
            .Stats
            .GetType()
            .GetProperties()
            .ToList()
            .Select(prop =>
            {
                var statVal = 0;
                try
                {
                    statVal = (int)prop.GetValue(item.Stats);
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

            var foundProp = props.Where(prop => prop.Name.Replace(propSuffix, "") == selectedStat.Item1);
            if (foundProp == null || !foundProp.Any())
                return item.DisplayName;
            else
            {
                var foundListMaybeNull = foundProp.FirstOrDefault();

                var foundList = ((List<string>)foundListMaybeNull.GetValue(this));

                return $"{item.DisplayName} of {foundList.ElementAt(Randomness.Instance.From(0, foundList.Count))}";
            }
        }
    }
}