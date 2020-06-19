namespace Apocalypse.Any.Domain.Common.Model.RPG
{
    public interface ICharacterSheet
    {
        int Attack { get; set; }
        int Defense { get; set; }
        int Health { get; set; }

        int Strength { get; set; }
        int Technology { get; set; }
        int Charisma { get; set; }
        int Speed { get; set; }
        int Aura { get; set; }

        int Experience { get; set; }

        int GetMaxAttributeValue();

        int GetMinAttributeValue();
    }
}