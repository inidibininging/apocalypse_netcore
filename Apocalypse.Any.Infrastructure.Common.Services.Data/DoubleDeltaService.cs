using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public class DoubleDeltaService : DeltaService<double>
    {
        protected override double GetDelta(double before, double after)
        {
            return after - before;
        }
    }
}
