using System;

namespace Apocalypse.Any.Domain.Common.Model.RPG
{
    public class CharacterSheet : ICharacterSheet
    {
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Health { get; set; }

        public int Strength { get; set; }
        public int Technology { get; set; }
        public int Charisma { get; set; }
        public int Speed { get; set; }
        public int Aura { get; set; }

        public int Experience { get; set; }
        public int Kills { get; set; }
        public int GetMaxAttributeValue()
        {
            return 100;
        }

        public int GetMinAttributeValue()
        {
            return 0;
        }

        #region Operators

        private const string OperatorNullException = "A CharacterSheet operator cannot be used with null";
        public static bool operator >=(CharacterSheet sheet, CharacterSheet sheet2) =>
            sheet2 == null ? throw new InvalidOperationException(OperatorNullException) :
            sheet.Attack >= sheet2.Attack &&
            sheet.Attack >= sheet2.Attack &&
            sheet.Defense >= sheet2.Defense &&
            sheet.Health >= sheet2.Health &&
            sheet.Strength >= sheet2.Strength &&
            sheet.Technology >= sheet2.Technology &&
            sheet.Charisma >= sheet2.Charisma &&
            sheet.Speed >= sheet2.Speed &&
            sheet.Aura >= sheet2.Aura &&
            sheet.Experience >= sheet2.Experience;

        public static bool operator <=(CharacterSheet sheet, CharacterSheet sheet2) =>
           sheet2 == null ? throw new InvalidOperationException(OperatorNullException) :
           sheet.Attack <= sheet2.Attack &&
           sheet.Attack <= sheet2.Attack &&
           sheet.Defense <= sheet2.Defense &&
           sheet.Health <= sheet2.Health &&
           sheet.Strength <= sheet2.Strength &&
           sheet.Technology <= sheet2.Technology &&
           sheet.Charisma <= sheet2.Charisma &&
           sheet.Speed <= sheet2.Speed &&
           sheet.Aura <= sheet2.Aura &&
           sheet.Experience <= sheet2.Experience;

        public static bool operator >(CharacterSheet sheet, CharacterSheet sheet2) =>
            sheet2 == null ? throw new InvalidOperationException(OperatorNullException) :
            sheet.Attack > sheet2.Attack &&
            sheet.Attack > sheet2.Attack &&
            sheet.Defense > sheet2.Defense &&
            sheet.Health > sheet2.Health &&
            sheet.Strength > sheet2.Strength &&
            sheet.Technology > sheet2.Technology &&
            sheet.Charisma > sheet2.Charisma &&
            sheet.Speed > sheet2.Speed &&
            sheet.Aura > sheet2.Aura &&
            sheet.Experience > sheet2.Experience;

        public static bool operator <(CharacterSheet sheet, CharacterSheet sheet2) =>
           sheet2 == null ? throw new InvalidOperationException(OperatorNullException) :
           sheet.Attack < sheet2.Attack &&
           sheet.Attack < sheet2.Attack &&
           sheet.Defense < sheet2.Defense &&
           sheet.Health < sheet2.Health &&
           sheet.Strength < sheet2.Strength &&
           sheet.Technology < sheet2.Technology &&
           sheet.Charisma < sheet2.Charisma &&
           sheet.Speed < sheet2.Speed &&
           sheet.Aura < sheet2.Aura &&
           sheet.Experience < sheet2.Experience;

        public static CharacterSheet operator *(CharacterSheet sheet, CharacterSheet sheet2)
        => sheet2 == null ? throw new InvalidOperationException(OperatorNullException) :
            new CharacterSheet()
        {
            Attack = sheet.Attack * sheet2.Attack,
            Defense = sheet.Defense * sheet2.Defense,
            Health = sheet.Health * sheet2.Health,
            Strength = sheet.Strength * sheet2.Strength,
            Technology = sheet.Technology * sheet2.Technology,
            Charisma = sheet.Charisma * sheet2.Charisma,
            Speed = sheet.Speed * sheet2.Speed,
            Aura = sheet.Aura * sheet2.Aura,
            Experience = sheet.Experience * sheet2.Experience
        };

        public static CharacterSheet operator /(CharacterSheet sheet, CharacterSheet sheet2)
        => sheet2 == null ? throw new InvalidOperationException(OperatorNullException) :
            new CharacterSheet()
        {
            Attack = sheet.Attack / sheet2.Attack,
            Defense = sheet.Defense / sheet2.Defense,
            Health = sheet.Health / sheet2.Health,
            Strength = sheet.Strength / sheet2.Strength,
            Technology = sheet.Technology / sheet2.Technology,
            Charisma = sheet.Charisma / sheet2.Charisma,
            Speed = sheet.Speed / sheet2.Speed,
            Aura = sheet.Aura / sheet2.Aura,
            Experience = sheet.Experience / sheet2.Experience
        };

        public static CharacterSheet operator +(CharacterSheet sheet, CharacterSheet sheet2)
        => sheet2 == null ? throw new InvalidOperationException(OperatorNullException) :
            new CharacterSheet()
        {
            Attack = sheet.Attack + sheet2.Attack,
            Defense = sheet.Defense + sheet2.Defense,
            Health = sheet.Health + sheet2.Health,
            Strength = sheet.Strength + sheet2.Strength,
            Technology = sheet.Technology + sheet2.Technology,
            Charisma = sheet.Charisma + sheet2.Charisma,
            Speed = sheet.Speed + sheet2.Speed,
            Aura = sheet.Aura + sheet2.Aura,
            Experience = sheet.Experience + sheet2.Experience
        };

        public static CharacterSheet operator -(CharacterSheet sheet, CharacterSheet sheet2)
        => sheet2 == null ? throw new InvalidOperationException(OperatorNullException) :
            new CharacterSheet()
        {
            Attack = sheet.Attack - sheet2.Attack,
            Defense = sheet.Defense - sheet2.Defense,
            Health = sheet.Health - sheet2.Health,
            Strength = sheet.Strength - sheet2.Strength,
            Technology = sheet.Technology - sheet2.Technology,
            Charisma = sheet.Charisma - sheet2.Charisma,
            Speed = sheet.Speed - sheet2.Speed,
            Aura = sheet.Aura - sheet2.Aura,
            Experience = sheet.Experience - sheet2.Experience
        };
        #endregion
    }
}