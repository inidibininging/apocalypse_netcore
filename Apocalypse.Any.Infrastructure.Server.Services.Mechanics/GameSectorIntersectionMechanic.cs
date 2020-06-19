using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class GameSectorIntersectionMechanic<TCharacter> : IGameSectorIntersectionMechanic<TCharacter> where TCharacter : CharacterEntity
    {
        private Action<TCharacter> OnIntersectionCallback { get; set; }

        public GameSectorIntersectionMechanic(
            Action<TCharacter> onIntersectionCallback
        )
        {
            if (onIntersectionCallback == null)
                throw new ArgumentNullException(nameof(onIntersectionCallback));
            OnIntersectionCallback = onIntersectionCallback;
        }

        public void Update(
            TCharacter target,
            IGameSectorBoundaries sectoBoundaries)
        {
            if (target.CurrentImage.Position.X >= sectoBoundaries.MaxSectorX)
            {
                OnIntersectionCallback(target);
            }
            if (target.CurrentImage.Position.X <= sectoBoundaries.MinSectorX)
            {
                OnIntersectionCallback(target);
            }
            if (target.CurrentImage.Position.Y >= sectoBoundaries.MaxSectorY)
            {
                OnIntersectionCallback(target);
            }
            if (target.CurrentImage.Position.Y <= sectoBoundaries.MinSectorY)
            {
                OnIntersectionCallback(target);
            }
        }
    }
}