using Apocalypse.Any.Core.Behaviour;
using System;

namespace Apocalypse.Any.Core.FXBehaviour
{
    public class FadeToBehaviour
    {
        public void Update(AlphaBehaviour alphaBehaviour, float destination, float fadeStep = 0.1f)
        {
            if (alphaBehaviour == null)
                throw new ArgumentNullException(nameof(alphaBehaviour));
            if (destination < 0)
                throw new ArgumentOutOfRangeException("fade to destination is below 0");
            if (destination > 1)
                throw new ArgumentOutOfRangeException("fade to destination is above 1");

            //if the step is somethign in between the source value and the optimal value. stop tweaking it.
            var valueDifference = (MathF.Abs(alphaBehaviour.Alpha - destination));
            var fadeStepAbs = MathF.Abs(fadeStep);
            if (MathF.Abs(valueDifference - fadeStepAbs) <= fadeStepAbs)
                alphaBehaviour.Alpha = destination;

            if (destination > alphaBehaviour.Alpha)
            {
                alphaBehaviour.Alpha += MathF.Abs(fadeStep);
            }

            if (destination < alphaBehaviour.Alpha)
            {
                alphaBehaviour.Alpha -= MathF.Abs(fadeStep);
            }
        }
    }
}