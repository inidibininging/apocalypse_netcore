using Apocalypse.Any.Core.Utilities;
using System;
using System.Linq;

namespace Apocalypse.Any.Domain.Common.Model.RPG
{
    public class CharacterSheetFactory
    {
        public ICharacterSheet GetDefaultStartSheet()
        {
            var characterSheet = new CharacterSheet();
            var props = typeof(CharacterSheet).GetProperties();
            props.ToList().ForEach(p => p.SetValue(characterSheet, characterSheet.GetMaxAttributeValue() / 2));
            return characterSheet;
        }

        public ICharacterSheet GetRandomSheet()
        {
            var characterSheet = new CharacterSheet();
            var props = typeof(CharacterSheet).GetProperties();

            props.ToList().ForEach(p =>
            {
                // Console.WriteLine($"generating randomness for {p.Name}");
                var randomValue = Randomness.Instance.From(characterSheet.GetMinAttributeValue(), characterSheet.GetMaxAttributeValue());
                // Console.WriteLine($"randomValue {randomValue}");
                p.SetValue(characterSheet, randomValue);
            });
            return characterSheet;
        }
    }
}