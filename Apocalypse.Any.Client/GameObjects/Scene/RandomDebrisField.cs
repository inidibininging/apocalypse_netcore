using Apocalypse.Any.Client.Mechanics;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class RandomDebrisField :
        GameObject,
        IVisualGameObject
    {
        private SpriteSheet Sheet { get; set; }

        public List<RandomDebris> RandomDebris { get; set; } = new List<RandomDebris>();
        public int DebrisCount { get; set; }

        public RandomDebrisField(int debrisCount = 100) : base()
        {
            DebrisCount = debrisCount;
        }

        public override void Initialize()
        {
            var frames = new Dictionary<string, Rectangle>();
            for (int i = 0; i < DebrisCount; i++)
            {
                var randomDebris = new RandomDebris();
                RandomDebris.Add(randomDebris);
                frames.Add(i.ToString(), new Rectangle(randomDebris.FramePosition.ToPoint(), new Point(32)));
            }
            Sheet = new SpriteSheet(frames) { Path = "Image/debris" };
            Add(new AsteroidScreenBehaviour(Sheet));
            base.Initialize();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var i = 0;
            RandomDebris.ForEach(randomDebris =>
            {
                Sheet.SelectedFrame = i.ToString();
                Sheet.Position.X = randomDebris.Position.X;
                Sheet.Position.Y = randomDebris.Position.Y;
                Sheet.Rotation.Rotation = randomDebris.Rotation.Rotation;
                Sheet.Color = randomDebris.Color;
                Sheet.Scale = randomDebris.Scale;
                Sheet.Draw(spriteBatch);
                i++;
            });
        }
        public void Add(float x,float y, Color color, float scale = 0.5f){
             //drop a piece of junk
            var droppedJunk = new RandomDebris();
            droppedJunk.Position = new MovementBehaviour(){
                X = x,
                Y = y,
            };
            droppedJunk.Scale = new Vector2(scale,scale);
            droppedJunk.Color = color;
            droppedJunk.ChangeDebrisFrame(100);
            RandomDebris.Add(droppedJunk);
            Sheet.SpriteSheetRectangle.Add(RandomDebris.Count.ToString(), new Rectangle(droppedJunk.FramePosition.ToPoint(), new Point(32)));
        }
        public override void Update(GameTime time)
        {
            RandomDebris.ForEach(randomAsteroid => randomAsteroid.Update(time));
            ForEach(obj => obj.Update(time));
        }
    }
}