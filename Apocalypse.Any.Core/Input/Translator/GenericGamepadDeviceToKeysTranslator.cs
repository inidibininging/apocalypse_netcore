using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Echse.Net.Domain;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class GenericGamepadDeviceToKeysTranslator :
        IInputTranslator<GamePadState, IEnumerable<string>>
    {
        public int GamePadIndex { get; private set; }

        public GenericGamepadDeviceToKeysTranslator() : this(0)
        {

        }
        public GenericGamepadDeviceToKeysTranslator(int gamePadIndex = 0)
        {
            GamePadIndex = gamePadIndex;
            
        }

        public IEnumerable<string> Translate(GamePadState input)
        {
            if (!input.IsConnected)
                return Array.Empty<string>();
            var commandInput = new List<string>();

            if (input.IsButtonDown(Buttons.A))
                commandInput.Add(DefaultKeys.Shoot);
            if (input.IsButtonDown(Buttons.B))
                commandInput.Add(DefaultKeys.AltShoot);
            if (input.IsButtonDown(Buttons.X))
                commandInput.Add(DefaultKeys.Boost);
            if (input.IsButtonDown(Buttons.Y))
                commandInput.Add(DefaultKeys.Defence);

            if (input.ThumbSticks.Left.X < -0.5f)
                commandInput.Add(DefaultKeys.Left);
            if (input.ThumbSticks.Left.X > 0.5f)
                commandInput.Add(DefaultKeys.Right);

            if (input.ThumbSticks.Left.Y < -0.5f)
                commandInput.Add(DefaultKeys.Down);
            if (input.ThumbSticks.Left.Y > 0.5f)
                commandInput.Add(DefaultKeys.Boost);

            return commandInput;
        }
    }
}