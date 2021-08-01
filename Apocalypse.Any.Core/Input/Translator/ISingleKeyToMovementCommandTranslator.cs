using Apocalypse.Any.Core.Behaviour;
using Echse.Net.Domain;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Input.Translator
{
    public interface ISingleKeyToMovementCommandTranslator :
        IInputTranslator<string, IList<ICommand<MovementBehaviour>>>
    {
    }
}