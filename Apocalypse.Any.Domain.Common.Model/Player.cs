namespace Apocalypse.Any.Domain.Common.Model
{
    public class Player : CharacterEntity
    {
        public string LoginToken { get; set; }
        public string ChosenStat { get; set; }

    }
}