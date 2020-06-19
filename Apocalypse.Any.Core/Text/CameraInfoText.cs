using Apocalypse.Any.Core.Camera;
using Apocalypse.Any.Core.Services;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Text
{
    /// <summary>
    /// This class shows basic info about the camera properties on the screen.
    /// </summary>
    public class CameraInfoText : GenericInfoText<ICamera>
    {
        public CameraInfoText(ICamera camera) : base(camera)
        {
        }

        /// <summary>
        /// This method returns a the info text position.
        /// The position can be still changed by changing the "Target"'s Position property after the parent's Update method block.
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetInfoTextPositionOnScreen()
        {
            Vector2 targetPosition = Target.Position;
            return new Vector2
                (
                targetPosition.X + (ScreenService.Instance.Resolution.X / 2),
                targetPosition.Y + (ScreenService.Instance.Resolution.Y / 4)
                );
        }

        public override string GetText()
            => $@"X:{Target.Position.X}
Y:{Target.Position.Y}
Rotation:{Target.Rotation.Rotation}
Zoom:{Target.Zoom}";
    }
}