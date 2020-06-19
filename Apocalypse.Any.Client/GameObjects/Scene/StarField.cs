using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apocalypse.Any.Client.GameObjects.Scene
{
    public class StarField : GameObject, IVisualGameObject
    {
        public StarField(int quantity = 20)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Apocalypse.Any cannot summon the stars upon you because you don't want to");
            Stars = new List<Star>();
            for (var i = 0; i < quantity; i++)
            {
                Stars.Add(new Star());
            }
        }

        private List<Star> Stars { get; }
        public IReadOnlyList<Star> StarsAvailable { get => Stars; }
        public int Quantity { get; set; }

        public override void LoadContent(ContentManager manager)
        {
            Stars.ForEach(star => star.LoadContent(manager));
            base.LoadContent(manager);
        }

        public override void UnloadContent()
        {
            Stars.ForEach(star => star.UnloadContent());
            base.UnloadContent();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Stars.ForEach(star => star.Draw(spriteBatch));
        }

        public override void Update(GameTime time)
        {
            //Parallel.ForEach(Stars, (star) => star.Update(time));
            Stars.ForEach(star =>
            {                
                //star.MoveRandomly();
                star.Update(time);
            });

            //TODO: do we need this?
            // this.ForEach(obj => obj.Update(time));
        }

        public void Add(float x = 0, float y = 0)
        {
            Stars.Add(new Star(x,y));
        }
    }
}