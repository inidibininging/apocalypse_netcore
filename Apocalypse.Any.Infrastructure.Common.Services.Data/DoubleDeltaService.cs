using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public class DoubleDeltaService : DeltaService<double>
    {
        protected override double GetDelta(double before, double after)
        {
            return after - before;
        }

        public override double GetMeanOfRecords()
        {
            if (!RecordDeltas || DeltaRecords.Count == 0)
                return 0;
            else
                return DeltaRecords.Average();
        }
    }
}
