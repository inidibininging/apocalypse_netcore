using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Client.GameObjects.Effects
{
    public class Spark : Image
    {
        public IUpdateableLite SparkOffTimer { get; set; }
        private TimeSpan SparkOffTimeSpan { get; set; }
        public bool KillMe { get; set; }

        public Spark() : base()
        {
            Path = "Image/Scene/Star256x256";
            float scale = 1 / Randomness.Instance.From(1, 1000);
            float randomScale = Convert.ToSingle(Randomness.Instance.From(10, 50)) / 100f;
            scale = randomScale;
            Scale = new Vector2(scale, scale);
            Color = new Color(Convert.ToSingle(Randomness.Instance.From(0, 255)), Convert.ToSingle(Randomness.Instance.From(0, 255)), Convert.ToSingle(Randomness.Instance.From(0, 255)));
            SparkOffTimeSpan = TimeSpan.FromMilliseconds(Randomness.Instance.From(5000, 10000));
            SparkOffTimer = SparkOffTimeSpan.Do(() => KillMe = true);
            Rotation.Rotation = Randomness.Instance.From(0, 365);
            KillMe = false;
        }

        public override void Update(GameTime time)
        {
            Rotation.Rotation = Randomness.Instance.From(0, 365);

            Alpha.Alpha -= (float)(time.ElapsedGameTime.TotalMilliseconds);

            var x = (float)(Math.Sin(Rotation));
            Position.X += x * Alpha.Alpha;

            var y = (float)(Math.Cos(Rotation)) * -1;
            Position.Y += y * Alpha.Alpha;

            SparkOffTimer.Update(time);
            base.Update(time);
        }
    }
}