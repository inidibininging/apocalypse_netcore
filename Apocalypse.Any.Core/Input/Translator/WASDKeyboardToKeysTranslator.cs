using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class WASDKeyboardToKeysTranslator : IInputTranslator<Keys[], IEnumerable<string>>
    {
        private const string UnknownKey = "null";

        private Dictionary<Keys, string> InternalKeyboardKeyMap { get; set; } = new Dictionary<Keys, string>()
        {
            [Keys.W] = DefaultKeys.Up,
            [Keys.S] = DefaultKeys.Down,
            [Keys.A] = DefaultKeys.Left,
            [Keys.D] = DefaultKeys.Right,
            [Keys.U] = DefaultKeys.Shoot,
            [Keys.I] = DefaultKeys.AltShoot,
            [Keys.O] = DefaultKeys.Defence,
            [Keys.Enter] = DefaultKeys.Continue,
            [Keys.LeftShift] = DefaultKeys.Boost,
            [Keys.I] = DefaultKeys.OpenInventory,
            [Keys.E] = DefaultKeys.Use,
            [Keys.Q] = DefaultKeys.Release,
            [Keys.D1] = DefaultKeys.ChooseHealth,
            [Keys.D2] = DefaultKeys.ChooseSpeed,
            [Keys.D3] = DefaultKeys.ChooseStrength,
            [Keys.D4] = DefaultKeys.ChooseArmor,
            [Keys.F11] = DefaultKeys.ToggleFullScreen,
        };

        public IEnumerable<string> Translate(Keys[] inputKeys = null) => (inputKeys ?? Keyboard.GetState().GetPressedKeys()).Select(key => (InternalKeyboardKeyMap.ContainsKey(key) ? InternalKeyboardKeyMap[key] : UnknownKey));
    }
}