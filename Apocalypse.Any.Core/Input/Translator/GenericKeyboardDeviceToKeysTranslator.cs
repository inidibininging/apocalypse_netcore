using Echse.Net.Domain;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class GenericKeyboardDeviceToKeysTranslator :
        IInputTranslator<Keys[], IEnumerable<string>>
    {
        private const string UnknownKey = "null";

        private Dictionary<Keys, string> InternalKeyboardKeyMap { get; set; } = new Dictionary<Keys, string>()
        {
            [Keys.W] = DefaultKeys.Up,
            [Keys.S] = DefaultKeys.Down,
            [Keys.A] = DefaultKeys.Left,
            [Keys.D] = DefaultKeys.Right,

            [Keys.Up] = DefaultKeys.Up,
            [Keys.Down] = DefaultKeys.Down,
            [Keys.Left] = DefaultKeys.Left,
            [Keys.Right] = DefaultKeys.Right
        };

        public IEnumerable<string> Translate(Keys[] inputKeys = null) => (inputKeys ?? Keyboard.GetState().GetPressedKeys()).Select(key => (InternalKeyboardKeyMap.ContainsKey(key) ? InternalKeyboardKeyMap[key] : UnknownKey));
    }
}