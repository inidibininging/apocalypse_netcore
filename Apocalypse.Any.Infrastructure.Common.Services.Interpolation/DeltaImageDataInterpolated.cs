using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Data;
using System;

namespace Apocalypse.Any.Infrastructure.Common.Services.Interpolation
{
    public class DeltaImageDataInterpolated
    {
        public string Id { get; set; }
        public FloatDeltaService X { get; set; } = new FloatDeltaService();
        public FloatDeltaService Y { get; set; } = new FloatDeltaService();
        public FloatDeltaService Alpha { get; set; } = new FloatDeltaService();
        public FloatDeltaService Rotation { get; set; } = new FloatDeltaService();
        public FloatDeltaService ScaleX { get; set; } = new FloatDeltaService();
        public FloatDeltaService ScaleY { get; set; } = new FloatDeltaService();
        public FloatDeltaService Width { get; set; } = new FloatDeltaService();
        public FloatDeltaService Height { get; set; } = new FloatDeltaService();

        public DeltaImageDataInterpolated Update(DeltaImageData imagesData)
        {
            Id = imagesData.Id;
            if (imagesData.X.HasValue)
                X.Update(imagesData.X.Value);
            if (imagesData.Y.HasValue)
                Y.Update(imagesData.Y.Value);
            if (imagesData.Alpha.HasValue)
                Alpha.Update(imagesData.Rotation.Value);
            if (imagesData.Rotation.HasValue)
                Rotation.Update(imagesData.Rotation.Value);
            if (imagesData.ScaleX.HasValue)
                ScaleX.Update(imagesData.ScaleX.Value);
            if (imagesData.ScaleY.HasValue)
                ScaleY.Update(imagesData.ScaleY.Value);
            if (imagesData.Width.HasValue)
                Width.Update(imagesData.Width.Value);
            if (imagesData.Height.HasValue)
                Height.Update(imagesData.Height.Value);
            return this;
        }
    }
}
