using Apocalypse.Any.Infrastructure.Common.Services.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Interpolation
{
    public class DeltaImageDataInterpolatedDelta : DeltaImageDataInterpolated
    {
        public DeltaImageDataInterpolatedDelta Update(DeltaImageDataInterpolated deltaImageDataInterpolated)
        {
            X.Update(deltaImageDataInterpolated.X.Delta);
            Y.Update(deltaImageDataInterpolated.Y.Delta);
            Rotation.Update(deltaImageDataInterpolated.Rotation.Delta);
            Alpha.Update(deltaImageDataInterpolated.Alpha.Delta);
            ScaleX.Update(deltaImageDataInterpolated.ScaleX.Delta);
            ScaleY.Update(deltaImageDataInterpolated.ScaleY.Delta);
            Width.Update(deltaImageDataInterpolated.Width.Delta);
            Height.Update(deltaImageDataInterpolated.Height.Delta);
            return this;
        }
    }
}
