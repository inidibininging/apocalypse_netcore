using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Apocalypse.Any.Core.Camera
{
    /// <summary>
    /// Top down camera class with even rotation. This only withholds a transformation matrix( I borrowed this from a xna tutorial (for now it's working)
    /// </summary>
    public class TopDownCamera :
        GameObject,
        ICamera,
        IMovableGameObject,
        IRotatableGameObject
    {
        public float Zoom { get; set; }

        public Viewport CurrentViewport { get; set; }

        public Matrix TransformMatrix
        {
            get
            {
                return
                    Matrix.CreateTranslation(
                        new Vector3(
                        -Position.X * ScreenService.Instance.Ratio.X,
                        -Position.Y * ScreenService.Instance.Ratio.Y, 0)
                        ) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(Zoom) *
                    Matrix.CreateTranslation(new Vector3(CurrentViewport.Width * 0.5f, CurrentViewport.Height * 0.5f, 0));
            }
        }

        public TopDownCamera(Viewport viewport)
        {
            CurrentViewport = viewport;

            Zoom = 1.0f;
            Position = new MovementBehaviour();
            Rotation = new RotationBehaviour();
        }

        private int deltaScrollWheelValue = 0;
        private int currentScrollWheelValue = 0;
        private TimeSpan delayed = TimeSpan.FromMilliseconds(200);
        private TimeSpan counter = TimeSpan.Zero;

        public CameraInfoText CameraDebugInfo { get; set; }
        public MovementBehaviour Position { get; set; }
        public RotationBehaviour Rotation { get; set; }

        public void Debug()
        {
            if (Position == null || Rotation == null)
                return;
            if (CameraDebugInfo == null)
                CameraDebugInfo = new CameraInfoText(this);
        }

        public override void Update(GameTime time)
        {
            if (time == null)
                return;

            if (counter < delayed)
            {
                counter += time.ElapsedGameTime;
                return;
            }
            else
            {
                deltaScrollWheelValue = 0;

                var nextScrollValue = Mouse.GetState().ScrollWheelValue;

                deltaScrollWheelValue = nextScrollValue - currentScrollWheelValue;
                currentScrollWheelValue += deltaScrollWheelValue;
                if (deltaScrollWheelValue != 0)
                {
                    var factor = currentScrollWheelValue / 120;

                    var nextZoom = (1 + ((float)factor / 10f));

                    if (nextZoom > 0.5 && nextZoom < 1.5) // prevent from "zooming" in reverse??
                        Zoom = MathHelper.Lerp(nextZoom,Zoom,0.06f);

                }

                counter = TimeSpan.Zero;
            }

            CameraDebugInfo?.Update(time);
        }
    }
}