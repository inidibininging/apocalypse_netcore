using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public class ByteDeltaService : DeltaService<byte>
    {
        protected override byte GetDelta(byte before, byte after)
        {            
            return (byte)(after - before);
        }
    }
}
