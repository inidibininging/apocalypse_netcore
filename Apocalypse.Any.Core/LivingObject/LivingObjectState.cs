using System;

namespace Apocalypse.Any.Core.LivingObject
{
    /// <summary>
    /// Before we try to get too philosophical here: The state of being alive or being dead is just a flag.
    /// </summary>
    [Flags]
    public enum LivingObjectState
    {
        Living = 1,
        Dead = 0
    }
}