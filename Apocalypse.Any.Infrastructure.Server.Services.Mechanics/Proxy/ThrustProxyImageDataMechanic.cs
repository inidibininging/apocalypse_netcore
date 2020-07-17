using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics;
using System;

namespace Apocalypse.Any.GameServer.Mechanics.Proxy
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