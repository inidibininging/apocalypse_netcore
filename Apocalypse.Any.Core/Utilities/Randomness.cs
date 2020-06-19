using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Core.Utilities
{
    public class Randomness
    {
        private static Randomness _instance;

        private Randomness()
        {
        }

        private Random Randomizer { get; } = new Random(DateTime.Now.Millisecond);

        public static Randomness Instance => _instance ?? (_instance = new Randomness());

        public int From(int minValue, int maxValue) => Randomizer.Next(minValue, maxValue);
        public bool TrueOrFalse() => new Random(DateTime.Now.Millisecond).Next(0, 2) == 1;

        public bool RollTheDice(int times)
        {
            var dices = new List<bool>();
            for (int diceTimes = 0; diceTimes < times; diceTimes++)
            {
                dices.Add(TrueOrFalse());
            }
            return dices.All<bool>((dice) => dice);
        }
    }
}