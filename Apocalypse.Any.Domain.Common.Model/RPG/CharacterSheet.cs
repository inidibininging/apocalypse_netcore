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

        public static CharacterSheet operator +(CharacterSheet sheet, CharacterSheet sheet2)
        => new CharacterSheet()
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
        => new CharacterSheet()
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
    }
}