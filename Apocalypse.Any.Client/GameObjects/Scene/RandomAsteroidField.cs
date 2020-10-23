using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class RandomAsteroidField :
        GameObject,
        IVisualGameObject
    {
        private SpriteSheet Sheet { get; set; }
        
        public List<RandomAsteroid> RandomAsteroids { get; set; } = new List<RandomAsteroid>();
        public int AsteroidCount { get; set; }

        public RandomAsteroidField(int asteroidCount = 50) : base()
        {
            AsteroidCount = asteroidCount;
        }

        public override void Initialize()
        {
            var frames = new Dictionary<(int asteroidNr, int dummyA, int dummyB), Rectangle>();
            for (int i = 0; i < AsteroidCount; i++)
            {
                var randomAsteroid = new RandomAsteroid();
                RandomAsteroids.Add(randomAsteroid);

                frames.Add((i, 0, 0), new Rectangle(randomAsteroid.FramePosition.ToPoint(), new Point(32)));
            }
            Sheet = new SpriteSheet(frames) { Path = ImagePaths.gamesheetExtended};

            //Add(new AsteroidScreenBehaviour(Sheet));
            base.Initialize();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var i = 0;
            RandomAsteroids.ForEach(randomAsteroid =>
            {
                Sheet.SelectedFrame = (i, 0, 0);
                Sheet.Position.X = randomAsteroid.Position.X;
                Sheet.Position.Y = randomAsteroid.Position.Y;
                Sheet.Rotation.Rotation = randomAsteroid.Rotation.Rotation;
                Sheet.Scale = randomAsteroid.Scale;
                Sheet.LayerDepth = randomAsteroid.LayerDepth;
                Sheet.Color = Color.BlueViolet;
                Sheet.Draw(spriteBatch);
                i++;
            });
        }
        private Task CalcMass;
        public override void Update(GameTime time)
        {
            // if(CalcMass == null)
            // {
            //     CalcMass = new Task(() => 
            //     {
            //         var field = RandomAsteroids;
            //         for (int a = 0; a < field.Count; a++)
            //         {
            //             for (int b = 0; b < field.Count; b++)
            //             {
            //                 var asteroidA = field[a];
            //                 var asteroidB = field[b];

            //                 var distance = Vector2.Distance(asteroidA.Position.ToVector2(),
            //                                                 asteroidB.Position.ToVector2());
            //                 if(distance > 5000)
            //                     continue;
            //                 Console.WriteLine(distance);
                            
            //                 // Console.WriteLine(asteroidA.Position.ToVector2().Length());
                            
            //                 double force = 1f;
            //                 try
            //                 {
            //                     force = (asteroidA.Mass * asteroidB.Mass) / System.Math.Pow(distance,3);
            //                 }
            //                 catch(Exception ex)
            //                 {
            //                     Console.WriteLine("ups");
            //                 }
                            
                            
            //                 Console.WriteLine("force "+ force);
                            
            //                 if(force > 20)
            //                     force = 0;

            //                 Thread.Sleep(0.05.Seconds());
            //                 var xDiff = asteroidB.Position.X - asteroidA.Position.X;
            //                 var yDiff = asteroidB.Position.Y - asteroidA.Position.Y;
            //                 var forceX = force * xDiff;
            //                 var forceY = force * yDiff;

            //                 asteroidA.Acceleration.X += (float)forceX / asteroidA.Mass;
            //                 asteroidA.Acceleration.Y += (float)forceY / asteroidA.Mass;

            //                 asteroidB.Acceleration.X -= (float)forceX / asteroidB.Mass;
            //                 asteroidB.Acceleration.Y -= (float)forceY / asteroidB.Mass;
            //             }
            //         }
            //         Console.WriteLine("-----------");
            //     });
            //     if(CalcMass.IsCompletedSuccessfully || CalcMass.Status == TaskStatus.Created){
            //         CalcMass.Start();
                    
            //     }
                    
            // }

            RandomAsteroids.ForEach(randomAsteroid => randomAsteroid.Update(time));
            //ForEach(obj => obj.Update(time));
        }
    }
}