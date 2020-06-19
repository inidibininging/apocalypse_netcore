namespace Apocalypse.Any.Domain.Common.Model
{
    public class Item : CharacterEntity
    {
        public string OwnerName { get; set; }
        public bool Taken { get; set; }
        public bool Used { get; set; }
        public bool Selected { get; set; }
        public bool InstantUse { get; set; }
        public int Order { get; set; }
    }
}