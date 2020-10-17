using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class ProductionBuilding<TCurrency> : EconomicCharacterEntity<TCurrency>
    {
        public TimeSpan ProductionTime { get; set; }
    }
}
