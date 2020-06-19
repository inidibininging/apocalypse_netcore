using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories
{
    public interface ICharacterNameGenerator<T>
        where T : CharacterEntity
    {
        string Generate(T entity);
    }
}