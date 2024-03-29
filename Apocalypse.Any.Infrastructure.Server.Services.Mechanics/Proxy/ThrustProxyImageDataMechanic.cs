using System;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy
{
    public class ThrustProxyImageDataMechanic
    : ISingleFullPositionHolderMechanic<ImageData>
    {
        public bool Active { get; set; } = true;
        private ThrustMechanic ThrustMechanics { get; set; }
        public float SpeedFactor { get; set; }

        public ThrustProxyImageDataMechanic(ThrustMechanic thrustMechanic)
        {
            ThrustMechanics = thrustMechanic ?? throw new ArgumentNullException(nameof(thrustMechanic));
        }

        public ImageData Update(ImageData entity)
        {
            ThrustMechanics.Update(entity, SpeedFactor);
            return entity;
        }
    }
}