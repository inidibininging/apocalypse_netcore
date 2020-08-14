using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data
{
    public class FloatDeltaServiceDeltaService : DeltaService<FloatDeltaService>
    {
        private FloatDeltaService FloatDeltaService { get; set; }
        public FloatDeltaServiceDeltaService()
        {
            FloatDeltaService = new FloatDeltaService();
        }
        protected override FloatDeltaService GetDelta(FloatDeltaService before, FloatDeltaService after)
        {
            FloatDeltaService.Update(before.Delta).Update(after.Delta);
            return FloatDeltaService;
        }
    }
}
