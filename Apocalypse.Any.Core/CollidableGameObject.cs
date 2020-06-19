using Apocalypse.Any.Core.Collision;
using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Apocalypse.Any.Core
{
    public abstract class CollidableGameObject<T> :
        GameObject,
        ICollidable,
        IVisualGameObject,
        IDeletableGameObject,
        IImageHolder<T> where T : AnimatedImage
    {
        private bool colliding = false;

        public bool Colliding
        {
            get
            {
                return colliding;
            }
            private set
            {
                colliding = value;
            }
        }

        public T CurrentImage { get; set; }

        public bool MarkedForDeletion { get; private set; }

        public override void UnloadContent()
        {
            CurrentImage?.UnloadContent();
            CurrentImage = null;
            base.UnloadContent();
            MarkedForDeletion = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentImage == null)
                return;
            CurrentImage.Draw(spriteBatch);
        }

        public Rectangle GetCurrentRectangle()
        {
            if (CurrentImage == null || MarkedForDeletion)
                return Rectangle.Empty;

            var croppedRect = CurrentImage.SourceRect;
            croppedRect.Width = (int)(Convert.ToSingle(croppedRect.Width) * CurrentImage.Scale.X);
            croppedRect.Height = (int)(Convert.ToSingle(croppedRect.Height) * CurrentImage.Scale.Y);
            croppedRect.Location = ((Vector2)CurrentImage.Position).ToPoint();
            return croppedRect;
        }

        public virtual void OnCollision(ICollidable collidable)
        {
            Colliding = true;
        }

        public override void Update(GameTime time)
        {
            ForEach(obj => obj.Update(time));
            if (Colliding)
                Colliding = false;
        }

        public void MarkForDeletion()
        {
            if (MarkedForDeletion)
                return;

            UnloadContent();
        }
    }
}