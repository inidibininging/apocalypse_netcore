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
        private  float ZoomDelta { get; set; }
        const float ZoomMinimum = 0.25f;
        const float ZoomMaximum = 2f;
        private const float ZoomSpeed = 0.025f;
        
        private const float ScrollZoomFactor = 0.50f;

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

        public void ZoomIn(float zoomFactor = ScrollZoomFactor)
        {
            if ((Zoom + ZoomDelta > ZoomMinimum && Zoom + ZoomDelta < ZoomMaximum))
                ZoomDelta = zoomFactor;
        }

        public void ZoomOut(float zoomFactor = ScrollZoomFactor)
        {
            if ((Zoom + ZoomDelta > ZoomMinimum && Zoom + ZoomDelta < ZoomMaximum))
                ZoomDelta = zoomFactor * -1;
        }

        public TopDownCamera(Viewport viewport)
        {
            CurrentViewport = viewport;

            Zoom = 1.0f;
            Position = new MovementBehaviour();
            Rotation = new RotationBehaviour();
        }

        private int scrollWheelValueRaw = 0;
        private int scrollWheelValueBefore = 0;
        private float NextZoom { get; set; }
        float ZoomDifference { get; set; }
        private readonly TimeSpan delayed = TimeSpan.FromMilliseconds(200);
        private TimeSpan _counter = TimeSpan.Zero;

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

            
            if (_counter < delayed)
            {
                _counter += time.ElapsedGameTime;
                return;
            }
            else
            {
                ZoomDifference = Math.Abs(NextZoom - Zoom);
                if (ZoomDifference >= 0.0004 && NextZoom != 0)
                {
                    Zoom = MathHelper.Lerp(Zoom, NextZoom, ZoomSpeed);
                }
                
                // if ((Zoom + ZoomDelta > ZoomMinimum && Zoom + ZoomDelta < ZoomMaximum))
                NextZoom = Zoom + ZoomDelta;
                
                ZoomDelta = 0;
                _counter = TimeSpan.Zero;
            }

            CameraDebugInfo?.Update(time);
        }
    }
}