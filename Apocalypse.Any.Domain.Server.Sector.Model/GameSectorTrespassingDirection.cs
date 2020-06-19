using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Sector.Model
{
    /// <summary>
    /// Describes in which direction the game sector is being trespassed
    /// </summary>
    [Flags]
    public enum GameSectorTrespassingDirection
    {                   
        Up = 1,
        Down = 2,
        Right = 4,
        Left = 8,
        None = 0,
    }
}
