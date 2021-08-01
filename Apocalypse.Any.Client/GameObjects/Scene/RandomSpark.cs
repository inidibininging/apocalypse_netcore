using System;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.DrawingOrder;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class RandomSpark :
        IMovableGameObject,
        IRotatableGameObject,
        IUpdateableLite
    {
        private readonly float RotationDirection;

        public RandomSpark()
        {
            Position.X = Randomness.Instance.From(0, (int) ScreenService.Instance.Resolution.X * 10);
            Position.Y = Randomness.Instance.From(0, (int) ScreenService.Instance.Resolution.Y * 10);
            RotationDirection = Randomness.Instance.TrueOrFalse() ? 1 : -1; //* Randomness.Instance.From(80, 100)/100;
            Destination.X = Randomness.Instance.From(0, (int) ScreenService.Instance.Resolution.X * 20);
            Destination.Y = Randomness.Instance.From(0, (int) ScreenService.Instance.Resolution.Y * 20);
            Color = Color.White;
            FramePosition = new Vector2(6 * 32, 7 * 32);

            // ChangeSparkFrame();
        }

        public Color Color { get; set; } = new(Randomness.Instance.From(100, 255),
            Randomness.Instance.From(100, 255),
            0);

        public Vector2 Scale { get; set; } = new(Randomness.Instance.From(0, 300) / 100f);
        public float LayerDepth { get; set; } = DrawingPlainOrder.EntitiesFX;
        private MovementBehaviour Destination { get; } = new();
        public float SingleUnit { get; set; } = 0.5f;

        public Vector2 FramePosition { get; set; }

        public string Path { get; } = "debris";

        public DateTime CreationDate { get; set; } = new();
        public TimeSpan DecayTime { get; set; } = TimeSpan.FromSeconds(15);

        public bool SparkDecayTime => CreationDate.Add(DecayTime) < DateTime.Now;

        public MovementBehaviour Position { get; set; } = new();
        public RotationBehaviour Rotation { get; set; } = new();

        public void Update(GameTime gameTime)
        {
            Rotation.Rotation += RotationDirection;
            var SingleUnitX = MathHelper.Lerp(Position.X, Destination.X, 0.0001f);
            var SingleUnitY = MathHelper.Lerp(Position.Y, Destination.Y, 0.0001f);
            Position.X = Destination.X > Position.X ? SingleUnitX : -SingleUnitX;
            Position.Y = Destination.Y > Position.Y ? SingleUnitY : -SingleUnitY;
        }

        private Vector2 GenerateRandomFrame(int rolls = 1)
        {
            return new(
                Randomness.Instance.From(0, 25) * 32,
                rolls == 1 ? Randomness.Instance.TrueOrFalse() ? 1 :
                0 :
                Randomness.Instance.RollTheDice(rolls) ? 1 : 0);
        }

        public void ChangeSparkFrame(int rolls = 1)
        {
            FramePosition = GenerateRandomFrame(rolls);
            // Console.WriteLine($"{FramePosition.X},{FramePosition.Y}");
        }
    }
}