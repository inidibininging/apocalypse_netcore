using Apocalypse.Any.Core.Behaviour;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class SingleKeyToRotationDelegateCommandTranslator : ISingleKeyToRotationCommandTranslator
    {
        private string Input { get; set; }
        private IList<ICommand<RotationBehaviour>> Rotations { get; set; }

        public SingleKeyToRotationDelegateCommandTranslator(string input, IList<ICommand<RotationBehaviour>> rotations)
        {
            Input = input;
            Rotations = rotations;
        }

        public IList<ICommand<RotationBehaviour>> Translate(string input)
        => Input == input ? Rotations : null;
    }
}