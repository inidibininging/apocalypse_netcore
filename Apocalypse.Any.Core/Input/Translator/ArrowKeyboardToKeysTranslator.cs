using Echse.Net.Domain;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class ArrowKeyboardToKeysTranslator :
        IInputTranslator<Keys[], IEnumerable<string>>
    {
        private const string UnknownKey = "null";

        private Dictionary<Keys, string> InternalKeyboardKeyMap { get; set; } = new Dictionary<Keys, string>()
        {
            [Keys.LeftControl] = DefaultKeys.Shoot,
            [Keys.LeftAlt] = DefaultKeys.AltShoot,
            [Keys.LeftShift] = DefaultKeys.Defence,

            [Keys.Up] = DefaultKeys.Up,
            [Keys.Down] = DefaultKeys.Down,
            [Keys.Left] = DefaultKeys.Left,
            [Keys.Right] = DefaultKeys.Right,

            [Keys.LeftShift] = DefaultKeys.Boost,
            [Keys.I] = DefaultKeys.OpenInventory,

            [Keys.C] = DefaultKeys.OpenCharacter,

            [Keys.F2] = DefaultKeys.OpenInfo,
            [Keys.F3] = DefaultKeys.CloseInfo,

            [Keys.E] = DefaultKeys.Use,
            [Keys.Q] = DefaultKeys.Release,
            [Keys.D1] = DefaultKeys.ChooseHealth,
            [Keys.D2] = DefaultKeys.ChooseSpeed,
            [Keys.D3] = DefaultKeys.ChooseStrength,
            [Keys.D4] = DefaultKeys.ChooseArmor,

            [Keys.F11] = DefaultKeys.ToggleFullScreen
        };

        public IEnumerable<string> Translate(Keys[] inputKeys = null) => (inputKeys ?? Keyboard.GetState().GetPressedKeys()).Select(key => (InternalKeyboardKeyMap.ContainsKey(key) ? InternalKeyboardKeyMap[key] : UnknownKey));
    }
}