using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Utilities;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class RandomPlanet :
        IMovableGameObject,
        IRotatableGameObject,
        IUpdateableLite
    {
        private readonly GameTimeTimer LoopScale = new(Randomness.Instance.From(2, 6).Seconds());


        public bool RotateRight;

        public RandomPlanet()
        {
            Position.X = Randomness.Instance.From(0, (int) ScreenService.Instance.Resolution.X * 8);
            Position.Y = Randomness.Instance.From(0, (int) ScreenService.Instance.Resolution.Y * 8);
            var size = Randomness.Instance.From(5, 40);
            Scale = new Vector2(size);
            Destination.X = Randomness.Instance.From(0, (int) ScreenService.Instance.Resolution.X * 8);
            Destination.Y = Randomness.Instance.From(0, (int) ScreenService.Instance.Resolution.Y * 8);
            RotateRight = Randomness.Instance.TrueOrFalse();
            Color = new Color
            (
                Randomness.Instance.From(100, 255),
                Randomness.Instance.From(100, 255),
                Randomness.Instance.From(100, 255)
            );
            LoopScale.Loop = true;
            //LoopScale.Do(() => Scale = Scale == new Vector2(3f) ? new Vector2(3.2f) : (Scale == new Vector2(3.2f) ? new Vector2(3f) : new Vector2(3.2f)));
        }

        public Vector2 Scale { get; set; } = new(Randomness.Instance.From(0, 300) / 100);
        private MovementBehaviour Destination { get; } = new();
        public float SingleUnit { get; set; } = 0.02f;

        public Color Color { get; set; }

        public Vector2 FramePosition { get; set; } = new(
            Randomness.Instance.From(0, 6) * 32,
            6 * 32);

        public MovementBehaviour Position { get; set; } = new();
        public RotationBehaviour Rotation { get; set; } = new();

        public void Update(GameTime gameTime)
        {
            float randomNr = Randomness.Instance.From(0, 50);
            if (RotateRight)
                Rotation.Rotation += randomNr / 10000;
            else
                Rotation.Rotation -= randomNr / 10000;

            Position.X += Destination.X > Position.X ? SingleUnit : -SingleUnit;
            Position.Y += Destination.Y > Position.Y ? SingleUnit : -SingleUnit;
            LoopScale.Update(gameTime);
        }
    }
}