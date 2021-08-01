using System;
using System.Collections.Generic;
using System.Linq;
using Echse.Net.Domain;

namespace Apocalypse.Any.Core.Input.Translator
{
    /// <summary>
    /// Records any input as pressed keys and returns any new input as pressed key and old input as released
    /// </summary>
    public class CommandPressReleaseTranslator : IInputTranslator<IEnumerable<string>, IEnumerable<string>>
    {
        private List<string> RecordedPressCommand { get; set; } = new List<string>();

        public IEnumerable<string> Translate(IEnumerable<string> input)
        {
            var newInputWithPress = input?.Count() == 0
                ? Array.Empty<string>()
                : input.Select(cmd => cmd + DefaultKeys.Press);

            //no recorded inputs => save input if not empty and return any keys with "+press"
            if(RecordedPressCommand.Count == 0 && input.Any()) {
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
