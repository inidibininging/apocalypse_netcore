using Apocalypse.Any.Client.Mechanics;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class RandomPlanetField :
        GameObject,
        IVisualGameObject
    {
        private SpriteSheet Sheet { get; set; }

        public List<RandomPlanet> RandomPlanets { get; set; } = new List<RandomPlanet>();
        public int PlanetCount { get; set; }

        public RandomPlanetField(int tentacleCount = 50) : base()
        {
            PlanetCount = tentacleCount;
        }

        public override void Initialize()
        {
            var frames = new Dictionary<(int asteroidNr, int dummyA, int dummyB), Rectangle>();
            for (int i = 0; i < PlanetCount; i++)
            {
                var randomTentacle = new RandomPlanet();
                RandomPlanets.Add(randomTentacle);
                frames.Add((i,0,0), new Rectangle(randomTentacle.FramePosition.ToPoint(), new Point(32)));
            }
            Sheet = new SpriteSheet(frames) { Path = ImagePaths.hud_misc_edit };
            Add(new AsteroidScreenBehaviour(Sheet));
            base.Initialize();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var i = 0;
            RandomPlanets.ForEach(randomAsteroid =>
            {
                Sheet.SelectedFrame = (i,0,0);
                Sheet.Position.X = randomAsteroid.Position.X;
                Sheet.Position.Y = randomAsteroid.Position.Y;
                Sheet.Rotation.Rotation = randomAsteroid.Rotation.Rotation;
                Sheet.Scale = randomAsteroid.Scale;
                Sheet.Color = randomAsteroid.Color;
                Sheet.Draw(spriteBatch);
                i++;
            });
        }

        public override void Update(GameTime time)
        {
            RandomPlanets.ForEach(randomAsteroid => randomAsteroid.Update(time));
            ForEach(obj => obj.Update(time));
        }
    }
}