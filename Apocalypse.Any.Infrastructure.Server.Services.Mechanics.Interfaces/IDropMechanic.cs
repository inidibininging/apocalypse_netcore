using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces
{
    public interface IDropMechanic
    {
        Item Update(CharacterEntity character, int offsetX, int offsetY);
    }
}