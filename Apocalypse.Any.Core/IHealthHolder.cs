using Apocalypse.Any.Core.Behaviour;

namespace Apocalypse.Any.Core
{
    /// <summary>
    /// This interface describes an object that contains health.
    /// </summary>
    public interface IHealthHolder : IUpdateableLite
    {
        HealthBehaviour Health { get; }
    }
}