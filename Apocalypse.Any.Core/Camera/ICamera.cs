using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Core.Camera
{
    /// <summary>
    /// Describes a game object that has the ability behave like a camera or is part of a camera.
    /// </summary>
    public interface ICamera : IMovableGameObject, IRotatableGameObject, IUpdateableLite
    {
        float Zoom { get; set; }
        Viewport CurrentViewport { get; set; }
        Matrix TransformMatrix { get; }
    }
}