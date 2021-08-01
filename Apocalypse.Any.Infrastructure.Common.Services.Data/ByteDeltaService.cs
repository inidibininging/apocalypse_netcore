using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public class ByteDeltaService : DeltaService<byte>
    {
        protected override byte GetDelta(byte before, byte after)
        {            
            return (byte)(after - before);
        }

        public override byte GetMeanOfRecords()
        {
            if (!RecordDeltas || DeltaRecords.Count == 0)
                return 0;
            else
                return (byte)(int)Math.Round(DeltaRecords.Average((b => b)));
        }
    }
}
