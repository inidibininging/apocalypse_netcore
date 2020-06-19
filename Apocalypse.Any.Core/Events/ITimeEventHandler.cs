using Apocalypse.Any.Core.Behaviour;

namespace Apocalypse.Any.Core.Events
{
    public interface ITimeEventHandler : IUpdateableLite
    {
        void Tick(TimeEventListenerBehaviour sender);
    }
}