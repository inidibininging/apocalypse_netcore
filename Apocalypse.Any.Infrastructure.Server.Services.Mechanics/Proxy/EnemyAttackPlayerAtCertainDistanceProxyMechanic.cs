using System;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.EnemyMechanics;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy
{
    public class EnemyAttackPlayerAtCertainDistanceProxyMechanic
    : ISingleEntityWithImageMechanic<EnemySpaceship>
    {
        public bool Active { get; set; } = true;
        public EnemyCreateProjectileIfPlayerIsNearMechanic EnemyCreateProjectileIfPlayerIsNearMechanic { get; set; }
        public EnemyAttackPlayerAtCertainDistanceProxyMechanic(EnemyCreateProjectileIfPlayerIsNearMechanic enemyCreateProjectileIfPlayerIsNearMechanic)
        {
            EnemyCreateProjectileIfPlayerIsNearMechanic = enemyCreateProjectileIfPlayerIsNearMechanic ?? throw new ArgumentNullException(nameof(enemyCreateProjectileIfPlayerIsNearMechanic));
        }
        public EnemySpaceship Update(EnemySpaceship singularEntity)
        {
            return null;
        }
    }
}
