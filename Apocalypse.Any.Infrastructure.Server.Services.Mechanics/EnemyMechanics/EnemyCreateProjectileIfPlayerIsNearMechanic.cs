using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations;
using Microsoft.Xna.Framework;
using System;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.EnemyMechanics
{
    public class EnemyCreateProjectileIfPlayerIsNearMechanic
    {
        private int DistanceForDisablingProjectiles { get; set; } = 512;
    

        private IGenericTypeFactory<Projectile> ProjectileFactory  { get; set; }

        public EnemyCreateProjectileIfPlayerIsNearMechanic(ProjectileFactory projectileFactory)
        {
            if (projectileFactory == null)
                throw new ArgumentNullException(nameof(projectileFactory));
            ProjectileFactory = projectileFactory;

        }

        public Projectile Update(
            EnemySpaceship enemy,
            PlayerSpaceship player,
            int offsetEnemy = 0,
            int offsetPlayer = 0)
        {
            if (enemy == null || player == null)
                return null;
            
            if (Vector2.Distance(player.CurrentImage.Position, enemy.CurrentImage.Position) <
                DistanceForDisablingProjectiles)
                return null;
            
            enemy.CurrentImage.Rotation.Rotation = MathHelper.Lerp(player.CurrentImage.Rotation, enemy.CurrentImage.Rotation, 0.001f);
            return ProjectileFactory.Create(enemy);

        }
    }
}