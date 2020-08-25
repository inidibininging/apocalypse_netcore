using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public class IntDeltaService : DeltaService<int>
    {
        protected override int GetDelta(int before, int after)
        {
            return after - before;
        }

        public override int GetMeanOfRecords()
        {
            if (RecordDeltas)
                return (int)Math.Round(DeltaRecords.Average());
            else
                return 0;
        }
    }
}
