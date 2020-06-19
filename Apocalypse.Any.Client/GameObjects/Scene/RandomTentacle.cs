using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Core.Utilities;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class RandomTentacle :
        IMovableGameObject,
        IRotatableGameObject,
        IUpdateableLite
    {
        public RotationBehaviour Rotation { get; set; } = new RotationBehaviour();
        public MovementBehaviour Position { get; set; } = new MovementBehaviour();

        public AlphaBehaviour Alpha { get; set; } = new AlphaBehaviour();
        public Vector2 Scale { get; set; } = new Vector2((float)(Randomness.Instance.From(0, 4500) / 100));
        private MovementBehaviour Destination { get; set; } = new MovementBehaviour();
        public float SingleUnit { get; set; } = 0.3f;
        private GameTimeTimer LoopScale = new GameTimeTimer(Randomness.Instance.From(2, 6).Seconds());

        public Vector2 FramePosition { get; set; } = new Vector2(
                    Randomness.Instance.From(0, 5) * 64,
                    Randomness.Instance.TrueOrFalse() ? 1 : 0);

        public string Path { get; private set; } = "Image/Map/Cthulu/CthuluTentacleSet00";

        public bool RotateRight = false;

        public RandomTentacle() : base()
        {
            Position.X = Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.X * 8);
            Position.Y = Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.Y * 8);
            Alpha.Alpha = (float)Randomness.Instance.From(0, 100) / 100;
            Destination.X = Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.X * 8);
            Destination.Y = Randomness.Instance.From(0, (int)ScreenService.Instance.Resolution.Y * 8);
            RotateRight = Randomness.Instance.TrueOrFalse();
            LoopScale.Loop = true;
            //LoopScale.Do(() => Scale = Scale == new Vector2(3f) ? new Vector2(3.2f) : (Scale == new Vector2(3.2f) ? new Vector2(3f) : new Vector2(3.2f)));
        }

        public void Update(GameTime gameTime)
        {
            float randomNr = Randomness.Instance.From(0, 200);
            if (RotateRight)
                Rotation.Rotation += (randomNr / 1000);
            else
                Rotation.Rotation -= (randomNr / 1000);

            Position.X += Destination.X > Position.X ? SingleUnit : -SingleUnit;
            Position.Y += Destination.Y > Position.Y ? SingleUnit : -SingleUnit;
            LoopScale.Update(gameTime);
        }
    }
}