using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.ProjectileMechanics;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy
{
    public class DestroyProjectileWithDecayTimeProxyMechanic : ISingleEntityWithImageMechanic<Projectile>
    {
        public TimeSpan DestroyTimeSpan { get; set; }
        private MarkProjectileWithDecayTimeAsDestroyedMechanic DestroyProjectileWithDecayTime { get; set; }

        public DestroyProjectileWithDecayTimeProxyMechanic(MarkProjectileWithDecayTimeAsDestroyedMechanic destroyProjectileWithDecayTime)
        {
            DestroyProjectileWithDecayTime = destroyProjectileWithDecayTime ?? throw new ArgumentNullException(nameof(destroyProjectileWithDecayTime));
        }

        public Projectile Update(Projectile singularEntity)
        {
            return DestroyProjectileWithDecayTime.Update(singularEntity, DestroyTimeSpan);
        }
    }
}