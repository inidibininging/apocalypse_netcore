using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public class IntDeltaService : DeltaService<int>
    {
        protected override int GetDelta(int before, int after)
        {
            return after - before;
        }
    }
}
