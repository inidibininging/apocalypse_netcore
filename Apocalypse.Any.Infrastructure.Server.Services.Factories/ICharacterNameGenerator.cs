using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories
{
    public interface ICharacterNameGenerator<in T>
        where T : CharacterEntity
    {
        string Generate(T entity);
    }
}