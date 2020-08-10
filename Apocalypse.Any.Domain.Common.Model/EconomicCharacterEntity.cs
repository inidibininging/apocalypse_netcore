using System;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class EconomicCharacterEntity<TCurrency> 
        : CharacterEntity, 
        IOwnable, 
        ICurrencyHolder<TCurrency>
    {
        public TimeSpan BuildTime { get; set; }
        public TimeSpan ProductionTime { get; set; }
        public TCurrency Amount { get; set; }
        public TCurrency Product { get; set; }
        public string OwnerName { get; set; }
    }
}