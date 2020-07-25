using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.EnemyMechanics
{
    public class EnemyFollowPlayerAtCertainDistanceMechanic
    {
        private ImageToRectangleTransformationService ImageToRectangle { get; set; }
        private FacePointMechanic FacePointMechanics { get; set; }
        private AttractionMechanic AttractionMechanics { get; set; }

        public EnemyFollowPlayerAtCertainDistanceMechanic(
            AttractionMechanic attractionMechanics,
            FacePointMechanic facePointMechanics,
            ImageToRectangleTransformationService imageToRectangleTransformationService
            )
        {
            if (attractionMechanics == null)
                throw new ArgumentNullException(nameof(attractionMechanics));
            AttractionMechanics = attractionMechanics;

            if (facePointMechanics == null)
                throw new ArgumentNullException(nameof(facePointMechanics));
            FacePointMechanics = facePointMechanics;

            if (imageToRectangleTransformationService == null)
                throw new ArgumentNullException(nameof(imageToRectangleTransformationService));
            ImageToRectangle = imageToRectangleTransformationService;
        }

        public void Update(
            EnemySpaceship enemy,
            PlayerSpaceship player,
            int offsetEnemy = 0,
            int offsetPlayer = 0,
            float force = 2)
        {
            if (enemy == null || player == null)
                return;

            var enemyRect = ImageToRectangle.TransformInRespectToCenter(enemy.CurrentImage, offsetEnemy);
            var playerRect = ImageToRectangle.TransformInRespectToCenter(player.CurrentImage, offsetPlayer);

            AttractionMechanics.Update(
            enemy.CurrentImage,
            player.CurrentImage,
            force);

            if (enemyRect.X > playerRect.X ||
             enemyRect.X + enemyRect.Width < playerRect.X ||
             enemyRect.Y > playerRect.Y ||
             enemyRect.Y + enemyRect.Height < playerRect.Y)
            {
                FacePointMechanics.Update(
                enemy.CurrentImage,
                player.CurrentImage);
            }
        }
    }
}