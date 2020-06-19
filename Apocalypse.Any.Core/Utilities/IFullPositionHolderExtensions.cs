using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Core.Utilities
{
    public static class IFullPositionHolderExtensions
    {
        public static Vector2 GetDirection(this IFullPositionHolder positionHolder) =>
            new Vector2(
                -(float)(Math.Sin(positionHolder.Rotation.Rotation)),
                (float)(Math.Cos(positionHolder.Rotation.Rotation)));

        public static Vector2 GetInvertedDirection(this IFullPositionHolder positionHolder) =>
            new Vector2(
                -(float)(Math.Sin(positionHolder.Rotation.Rotation)),
                (float)(Math.Cos(positionHolder.Rotation.Rotation)) * -1);
    }
}