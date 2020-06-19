using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.PositionMechanics
{
    public class RandomRotationMechanic
        : ISingleFullPositionHolderMechanic<IFullPositionHolder>,
        IRandomRotationMechanic
    {
        public IFullPositionHolder Update(IFullPositionHolder target)
        {
            var doRotation = Convert.ToSingle(Randomness.Instance.From(0, 100)) / 100f > 0.5;

            if (!doRotation)
                return target;

            var triggerFired = Convert.ToSingle(Randomness.Instance.From(0, 100)) / 100f > 0.5;
            if (triggerFired)
                target.Rotation.Rotation += 1;
            else
                target.Rotation.Rotation -= 1;

            return target;
        }
    }
}