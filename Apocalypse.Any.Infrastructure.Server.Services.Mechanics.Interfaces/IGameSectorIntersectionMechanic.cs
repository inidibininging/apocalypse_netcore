using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces
{
    public interface IGameSectorIntersectionMechanic<TCharacter> where TCharacter : CharacterEntity
    {
        void Update(TCharacter target, IGameSectorBoundaries sectorBoundaries);
    }
}