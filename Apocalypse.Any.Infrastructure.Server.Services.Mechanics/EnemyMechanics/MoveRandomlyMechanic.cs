using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.PositionMechanics;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.EnemyMechanics
{
    public class MoveRandomlyMechanic
    : ISingleCharacterEntityMechanic<EnemySpaceship>
    {
        public bool Active { get; set; } = true;
        private ThrustMechanic ThrustMechanics { get; set; }
        private RandomRotationMechanic RandomRotationMechanic { get; set; }

        public MoveRandomlyMechanic(
            ThrustMechanic thrustMechanics,
            RandomRotationMechanic randomRotationMechanic
        )
        {
            ThrustMechanics = thrustMechanics ?? throw new ArgumentNullException(nameof(ThrustMechanics));
            RandomRotationMechanic = randomRotationMechanic ?? throw new ArgumentNullException(nameof(RandomRotationMechanic));
        }

        private float CalculateRandomFactor() => ((float)Randomness.Instance.From(150, 200) / 100f);

        public EnemySpaceship Update(EnemySpaceship enemy)
        {
            var factor = CalculateRandomFactor();
            ThrustMechanics.Update(enemy.CurrentImage, factor);
            RandomRotationMechanic.Update(enemy.CurrentImage);
            return enemy;
        }
    }
}