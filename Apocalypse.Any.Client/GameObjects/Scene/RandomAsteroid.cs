using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class RandomAsteroid :
        IMovableGameObject,
        IRotatableGameObject,
        IUpdateableLite
    {
        public RotationBehaviour Rotation { get; set; } = new RotationBehaviour();
        public MovementBehaviour Position { get; set; } = new MovementBehaviour();
        public Vector2 Scale { get; set; } = new Vector2((float)(Randomness.Instance.From(10, 200) / 150));
        private MovementBehaviour Destination { get; } = new MovementBehaviour();
        public float Mass => 1000f*Scale.X;
        public float SingleUnit { get; set; } = 0.5f;
        public float SingleUnitDepth { get; set; } = 0.001f;

        public Vector2 FramePosition { get; set; }

        public string Path { get; } = "gamesheetExtended";

        public bool RotateRight = false;

        public float LayerDepth { get; set; }
        

        public Vector2 Acceleration = Vector2.Zero;
        public Vector2 Speed = Vector2.Zero;
        public RandomAsteroid() : base()
        {
            Position.X = Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.X * 48);
            Position.Y = Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.Y * 48);
            var nextX = Randomness.Instance.From(0, 8) * 32;
            var nextY = Randomness.Instance.From(3, 5) * 32;
            FramePosition = new Vector2(nextX, nextY);
            Destination.X = Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.X * 48);
            Destination.Y = Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.Y * 48);
            LayerDepth = Randomness.Instance.TrueOrFalse() ? DrawingPlainOrder.Entities - DrawingPlainOrder.PlainStep : DrawingPlainOrder.Entities + DrawingPlainOrder.MicroPlainStep;
            RotateRight = Randomness.Instance.TrueOrFalse();
        }
        private bool GoUp = Randomness.Instance.From(0,100) > 50;
        public void Update(GameTime gameTime)
        {
            if (RotateRight)
                Rotation.Rotation += 1;
            else
                Rotation.Rotation -= 1;

            //Speed.X += Acceleration.X;
            //Speed.Y += Acceleration.Y;
            Position.X += Destination.X > Position.X ? SingleUnit : -SingleUnit;//Speed.X;//Destination.X > Position.X ? SingleUnit : -SingleUnit;//Speed.X;//Destination.X > Position.X ? SingleUnit : -SingleUnit;
            Position.Y += Destination.Y > Position.Y ? SingleUnit : -SingleUnit;//Speed.Y;//Destination.Y > Position.Y ? SingleUnit : -SingleUnit;//Speed.Y;//Destination.Y > Position.Y ? SingleUnit : -SingleUnit;            
            Scale = new Vector2(Scale.X + (SingleUnitDepth *(GoUp ? 1 : -1)),
                                Scale.Y + (SingleUnitDepth *(GoUp ? 1 : -1)));

            if(Scale.X < 0.1 && Randomness.Instance.From(0,100) > 50)
                GoUp = true;            
            if(Scale.X > 4 && Randomness.Instance.From(0,100) > 50)
                GoUp = false;

        }
    }
}