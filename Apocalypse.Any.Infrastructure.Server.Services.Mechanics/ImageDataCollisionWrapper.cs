using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Collision;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations;
using Microsoft.Xna.Framework;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class ImageDataCollisionWrapper<TCharacter> : GameObject,
        IEntityWithImageCollidable<TCharacter>
        where TCharacter : IEntityWithImage
    {
        private bool colliding = false;

        public bool Colliding
        {
            get { return colliding; }
            set { colliding = value; }
        }

        public TCharacter CharacterEntity { get; private set; }
        private Action<TCharacter> OnCollisionCallback { get; set; }

        private ImageToRectangleTransformationService ImageToRectangle { get; set; }

        public ImageDataCollisionWrapper(
            TCharacter characterEntity,
            Action<TCharacter> onCollisionCallback,
            ImageToRectangleTransformationService imageToRectangleTransformationService
        )
        {
            if (characterEntity == null)
                throw new ArgumentNullException(nameof(characterEntity));
            CharacterEntity = characterEntity;

            if (onCollisionCallback == null)
                throw new ArgumentNullException(nameof(onCollisionCallback));
            OnCollisionCallback = onCollisionCallback;

            if (imageToRectangleTransformationService == null)
                throw new ArgumentNullException(nameof(imageToRectangleTransformationService));
            ImageToRectangle = imageToRectangleTransformationService;
        }

        public Rectangle GetCurrentRectangle() => ImageToRectangle.Transform(CharacterEntity.CurrentImage);

        public void OnCollision(ICollidable collidable)
        {
            //Console.WriteLine(collidable.GetType().FullName);

            if (collidable.Equals(this))
                return;

            if ((collidable as ImageDataCollisionWrapper<TCharacter>).CharacterEntity.Equals(this.CharacterEntity))
                return;

            Colliding = true;
            OnCollisionCallback?.Invoke((collidable as IEntityWithImageCollidable<TCharacter>).CharacterEntity);
            Colliding = false;
        }

        public override void Update(GameTime time)
        {
            throw new NotImplementedException();
        }
    }
}