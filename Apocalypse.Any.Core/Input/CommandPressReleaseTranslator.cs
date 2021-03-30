using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Core.Input.Translator;

namespace Apocalypse.Any.Core.Input
{
    /// <summary>
    /// Records any given input as pressed keys and old input as released
    /// </summary>
    public class CommandPressReleaseTranslator : IInputTranslator<IEnumerable<string>, IEnumerable<string>>
    {
        private List<string> RecordedPressCommand { get; set; } = new();

        /// <summary>
        /// Translates any input into input + press key and any old input that is not given as my_old_input + released
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IEnumerable<string> Translate(IEnumerable<string> input)
        {
            
            var newInputWithPress = input?.Count() == 0
                ? Array.Empty<string>()
                : input.Select(cmd => cmd + DefaultKeys.Press);

            //no recorded inputs => save input if not empty and return any keys with "+press"
            if(RecordedPressCommand.Count == 0 && input.Any())
            {
                //doubled to list because we dont want to have the recorded press commands modified outside of this class
                return RecordedPressCommand = newInputWithPress.ToList();
            }

            //convert any press keys with release 
            //DefaultKeys CANNOT contain other commands : press or release. A TODO
            var keysNotInNewInput = RecordedPressCommand.Except(newInputWithPress);
            var releasedKeys = keysNotInNewInput.Select(cmd => cmd.Replace(DefaultKeys.Press, DefaultKeys.Release));
            
            //pressed keys are the same
            if(!releasedKeys.Any()) {
                return Array.Empty<string>();
            }

            //save new inputs
            RecordedPressCommand = newInputWithPress.Except(keysNotInNewInput).ToList();

            //merge pressed and released keys into a single list
            var result =  RecordedPressCommand.ToList();
            result.AddRange(releasedKeys);
            
            return result;
        }
    }
}
