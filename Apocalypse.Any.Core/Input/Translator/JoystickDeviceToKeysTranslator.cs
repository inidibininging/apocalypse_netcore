using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class JoystickDeviceToKeysTranslator
        : IInputTranslator<JoystickState, IEnumerable<string>>
    {
        public int JoystickIndex { get; private set; }

        public JoystickDeviceToKeysTranslator(int joystickIndex = 0)
        {
            JoystickIndex = joystickIndex;
        }

        public IEnumerable<string> Translate(JoystickState input)
        {
            
            if (!input.IsConnected)
                return Array.Empty<string>();
            
            var commandInput = new List<string>();
            foreach (var axe in input.Axes)
                Console.WriteLine(axe);

            //if (input.Buttons(Buttons.A))
            //    commandInput.Add(DefaultKeys.Shoot);
            //if (input.IsButtonDown(Buttons.B))
            //    commandInput.Add(DefaultKeys.AltShoot);
            //if (input.IsButtonDown(Buttons.X))
            //    commandInput.Add(DefaultKeys.Boost);
            //if (input.IsButtonDown(Buttons.Y))
            //    commandInput.Add(DefaultKeys.Defence);

            //if (input.ThumbSticks.Left.X < -0.5f)
            //    commandInput.Add(DefaultKeys.Left);
            //if (input.ThumbSticks.Left.X > 0.5f)
            //    commandInput.Add(DefaultKeys.Right);

            //if (input..Left.Y < -0.5f)
            //    commandInput.Add(DefaultKeys.Up);
            //if (input.ThumbSticks.Left.Y > 0.5f)
            //    commandInput.Add(DefaultKeys.Down);

            var xIndex = 0;
            var yIndex = 1;

            //if (input. < -0.5f)
            //    commandInput.Add(DefaultKeys.Left);
            //if (input.ThumbSticks.Left.X > 0.5f)
            //    commandInput.Add(DefaultKeys.Right);

            //if (input..Left.Y < -0.5f)
            //    commandInput.Add(DefaultKeys.Up);
            //if (input.ThumbSticks.Left.Y > 0.5f)
            //    commandInput.Add(DefaultKeys.Down);
            return commandInput;
        }
    }
}