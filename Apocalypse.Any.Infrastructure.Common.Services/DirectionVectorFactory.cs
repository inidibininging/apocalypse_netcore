using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Input;
using Echse.Net.Domain;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Infrastructure.Common.Services
{
    /// <summary>
    /// Creates a direction vector (Vector2), based on the rotation of the object
    /// </summary>
    public class DirectionVectorFactory : IInputTranslator<IFullPositionHolder, Vector2>
    {
        public Vector2 Translate(IFullPositionHolder positionHolder)
        {
            var x = (float)(MathF.Sin(positionHolder.Rotation));
            var y = (float)(MathF.Cos(positionHolder.Rotation)) * -1;
            return new Vector2(x, y);
        }
    }
}