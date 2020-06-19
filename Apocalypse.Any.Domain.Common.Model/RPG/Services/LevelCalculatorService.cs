using System;

namespace Apocalypse.Any.Domain.Common.Model.RPG.Services
{
    public class LevelCalculatorService
    {
        //POKEMON!
        public double GetLevel(double level)
        {
            return Math.Round((4 * (Math.Pow(level, 3))) / 5);
        }
    }
}