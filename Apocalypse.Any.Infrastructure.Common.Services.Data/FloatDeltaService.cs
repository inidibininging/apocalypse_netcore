using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public class FloatDeltaService : DeltaService<float>
    {
        
        protected override float GetDelta(float before, float after)
        {
            return after - before;
        }

        public override float GetMeanOfRecords()
        {
            if (!RecordDeltas || DeltaRecords.Count == 0)
                return 0;
            else
                return DeltaRecords.Average();
        }
    }
}
