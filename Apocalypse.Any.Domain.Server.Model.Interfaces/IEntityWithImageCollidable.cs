using Apocalypse.Any.Core.Collision;
using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    public interface IEntityWithImageCollidable<TEntityWithImage>
        : ICollidable
        where TEntityWithImage : IEntityWithImage
    {
        TEntityWithImage CharacterEntity { get; }
    }
}