using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public class FloatDeltaService : DeltaService<float>
    {
        protected override float GetDelta(float before, float after)
        {
            return after - before;
        }
    }
}
