using Apocalypse.Any.Core.Model;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class Item : CharacterEntity, IIdentifiableModel
    {
        public string Id { get; set; }
        public string OwnerName { get; set; }
        public bool Taken { get; set; }
        public bool Used { get; set; }
        public bool Selected { get; set; }
        public bool InstantUse { get; set; }
        public int Order { get; set; }
    }
}