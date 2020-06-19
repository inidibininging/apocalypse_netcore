using Apocalypse.Any.Client.Mechanics;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class RandomTentacleField :
        GameObject,
        IVisualGameObject
    {
        private SpriteSheet Sheet { get; set; }

        public List<RandomTentacle> RandomAsteroids { get; set; } = new List<RandomTentacle>();
        public int TentacleCount { get; set; }

        public RandomTentacleField(int tentacleCount = 100) : base()
        {
            TentacleCount = tentacleCount;
        }

        public override void Initialize()
        {
            var frames = new Dictionary<string, Rectangle>();
            for (int i = 0; i < TentacleCount; i++)
            {
                var randomTentacle = new RandomTentacle();
                RandomAsteroids.Add(randomTentacle);
                frames.Add(i.ToString(), new Rectangle(randomTentacle.FramePosition.ToPoint(), new Point(64)));
            }
            Sheet = new SpriteSheet(frames) { Path = "Image/Map/Cthulu/CthuluTentacleSet00" };
            Add(new AsteroidScreenBehaviour(Sheet));
            base.Initialize();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var i = 0;
            RandomAsteroids.ForEach(randomAsteroid =>
            {
                Sheet.SelectedFrame = i.ToString();
                Sheet.Position.X = randomAsteroid.Position.X;
                Sheet.Position.Y = randomAsteroid.Position.Y;
                Sheet.Rotation.Rotation = randomAsteroid.Rotation.Rotation;
                Sheet.Scale = randomAsteroid.Scale;
                Sheet.Color = Color.SeaGreen;
                Sheet.Alpha = randomAsteroid.Alpha;
                Sheet.Draw(spriteBatch);
                i++;
            });
        }

        public override void Update(GameTime time)
        {
            RandomAsteroids.ForEach(randomAsteroid => randomAsteroid.Update(time));
            ForEach(obj => obj.Update(time));
        }
    }
}