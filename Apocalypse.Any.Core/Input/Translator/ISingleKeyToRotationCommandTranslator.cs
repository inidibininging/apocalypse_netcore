using Apocalypse.Any.Core.Behaviour;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Input.Translator
{
    public interface ISingleKeyToRotationCommandTranslator :
        IInputTranslator<string, IList<ICommand<RotationBehaviour>>>
    {
    }
}