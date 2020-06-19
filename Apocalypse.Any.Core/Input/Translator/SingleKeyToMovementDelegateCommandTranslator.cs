using Apocalypse.Any.Core.Behaviour;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class SingleKeyToMovementDelegateCommandTranslator : ISingleKeyToMovementCommandTranslator
    {
        private string Input { get; set; }
        private IList<ICommand<MovementBehaviour>> Movements { get; set; }

        public SingleKeyToMovementDelegateCommandTranslator(string input, IList<ICommand<MovementBehaviour>> movements)
        {
            Input = input;
            Movements = movements;
        }

        public IList<ICommand<MovementBehaviour>> Translate(string input)
        => Input == input ? Movements : null;
    }
}