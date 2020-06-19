using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.EnemyMechanics
{
    public class EnemyCreateProjectileIfPlayerIsNearMechanic
    {
        private ProjectileFactory ProjectileMaker  { get; set; }
        // private ImageToRectangleTransformationService ImageToRectangle { get; set; }

        public EnemyCreateProjectileIfPlayerIsNearMechanic(
            ProjectileFactory projectileFactory//,
            // ImageToRectangleTransformationService imageToRectangleTransformationService
            )
        {
            if (projectileFactory == null)
                throw new ArgumentNullException(nameof(projectileFactory));
            ProjectileMaker = projectileFactory;

            // if (imageToRectangleTransformationService == null)
            //     throw new ArgumentNullException(nameof(imageToRectangleTransformationService));
            // ImageToRectangle = imageToRectangleTransformationService;
        }

        public Projectile Update(
            EnemySpaceship enemy,
            PlayerSpaceship player,
            int offsetEnemy = 0,
            int offsetPlayer = 0)
        {
            if (enemy == null || player == null)
                return null;
            var distance = 512;
            var distanceBetweenPlayerAndEnemy = Vector2.Distance(player.CurrentImage.Position, enemy.CurrentImage.Position);
            if(distanceBetweenPlayerAndEnemy > distance)
                return null;
            enemy.CurrentImage.Rotation.Rotation = MathHelper.Lerp(player.CurrentImage.Rotation, enemy.CurrentImage.Rotation, 0.001f);
            return ProjectileMaker.Create(enemy);
            // var enemyRect = ImageToRectangle.Transform(enemy.CurrentImage, offsetEnemy);
            // var playerRect = ImageToRectangle.Transform(player.CurrentImage, offsetPlayer);

        }
    }
}