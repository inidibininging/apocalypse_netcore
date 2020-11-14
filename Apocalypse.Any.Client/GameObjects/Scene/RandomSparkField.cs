using Apocalypse.Any.Client.Mechanics;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Constants;
using Apocalypse.Any.Domain.Common.DrawingOrder;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class RandomSparkField :
        GameObject,
        IVisualGameObject
    {
        private SpriteSheet Sheet { get; set; }

        public List<RandomSpark> RandomSparks { get; set; } = new List<RandomSpark>();
        public int SparksCount { get; set; }

        public RandomSparkField(int sparksCount = 100) : base()
        {
            SparksCount = sparksCount;
        }

        public override void Initialize()
        {
            var frames = new Dictionary<(int asteroidNr, int dummyA, int dummyB), Rectangle>();
            Sheet = new SpriteSheet(frames) { Path = ImagePaths.gamesheetExtended };
            Add(new AsteroidScreenBehaviour(Sheet));
            base.Initialize();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var i = 0;
            RandomSparks.ForEach(randomSpark =>
            {
                Sheet.SelectedFrame = (i, 0 ,0 );
                Sheet.Position.X = randomSpark.Position.X;
                Sheet.Position.Y = randomSpark.Position.Y;
                Sheet.Rotation.Rotation = randomSpark.Rotation.Rotation;
                Sheet.Color = randomSpark.Color;
                Sheet.Scale = randomSpark.Scale;
                Sheet.LayerDepth = randomSpark.LayerDepth;
                Sheet.Draw(spriteBatch);
                i++;
            });
        }
        public void Add(float x,float y, Color color,float scale = 0.5f, float layerDepth = DrawingPlainOrder.EntitiesFX){
            var spark = new RandomSpark();
            spark.Position = new MovementBehaviour(){
                X = x,
                Y = y,
            };
            spark.Scale = new Vector2(scale,scale);
            spark.Color = color;
            spark.LayerDepth = layerDepth;
            // spark.ChangeSparkFrame(100);
            RandomSparks.Add(spark);
            Sheet.SpriteSheetRectangle.Add((RandomSparks.Count, 0, 0), new Rectangle(spark.FramePosition.ToPoint(), new Point(32)));
        }
        public override void Update(GameTime time)
        {
            RandomSparks.ForEach(randomSpark => randomSpark.Update(time));
            ForEach(obj => obj.Update(time));
        }
    }
}