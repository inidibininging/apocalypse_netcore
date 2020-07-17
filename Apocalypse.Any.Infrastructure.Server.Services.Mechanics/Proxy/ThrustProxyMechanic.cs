using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics;
using System;

namespace Apocalypse.Any.GameServer.Mechanics.Proxy
{
    public class ThrustProxyMechanic
    : ISingleFullPositionHolderMechanic<IFullPositionHolder>
    {
        public bool Active { get; set; } = true;
        private ThrustMechanic ThrustMechanics { get; set; }
        public float SpeedFactor { get; set; }

        public ThrustProxyMechanic(ThrustMechanic thrustMechanic)
        {
            if (thrustMechanic == null)
                throw new ArgumentNullException(nameof(thrustMechanic));
            ThrustMechanics = thrustMechanic;
        }

        public IFullPositionHolder Update(IFullPositionHolder entity)
        {            
            return ThrustMechanics.Update(entity, SpeedFactor);
        }
    }
}