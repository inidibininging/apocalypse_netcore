using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.ProjectileMechanics
{
    public class MarkProjectileWithDecayTimeAsDestroyedMechanic
    {
        public Projectile Update(Projectile projectile, TimeSpan additiveDecayTime)
        {
            if (projectile.Destroyed)
                return projectile;

            if (projectile.CreationTime.Add(projectile.DecayTime) < DateTime.Now)
                projectile.Destroyed = true;

            return projectile;
        }

        public Projectile Update(Projectile projectile) => Update(projectile, 0.Seconds());
    }
}