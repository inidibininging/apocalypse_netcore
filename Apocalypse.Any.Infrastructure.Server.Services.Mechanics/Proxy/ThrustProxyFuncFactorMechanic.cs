using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Mechanics;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy
{
    public class ThrustProxyFuncFactorMechanic<T>
        : ISingleFullPositionHolderMechanic<IFullPositionHolder>        
    {
        public bool Active { get; set; } = true;
        private ThrustMechanic ThrustMechanics { get; set; }
        private Func<float> GetFactor { get; set; }

        public ThrustProxyFuncFactorMechanic(ThrustMechanic thrustMechanic, Func<float> getFactor)
        {
            if (thrustMechanic == null)
                throw new ArgumentNullException(nameof(thrustMechanic));
            ThrustMechanics = thrustMechanic;
            if (getFactor == null)
                throw new ArgumentNullException(nameof(getFactor));
            GetFactor = getFactor;
        }

        public IFullPositionHolder Update(IFullPositionHolder entity)
        {
            return ThrustMechanics.Update(entity, GetFactor());
        }

    }
}