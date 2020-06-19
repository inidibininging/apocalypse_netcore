using Apocalypse.Any.Core.LivingObject;

namespace Apocalypse.Any.Core
{
    public interface ILivingObject : IHealthHolder, IGameObject
    {
        LivingObjectState GetLivingState();
    }
}