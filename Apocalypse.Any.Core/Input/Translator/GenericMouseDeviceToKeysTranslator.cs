using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Input.Translator
{
    public class GenericMouseDeviceToKeysTranslator :
        IInputTranslator<MouseState, IEnumerable<string>>
    {
        private int deltaScrollWheelValue = 0;
        private int currentScrollWheelValue = 0;
        private float zoom { get; set; }

        public IEnumerable<string> Translate(MouseState input)
        {
            var commandInput = new List<string>();

            if (input.LeftButton == ButtonState.Pressed)
                commandInput.Add(DefaultKeys.Shoot);

            if (input.RightButton == ButtonState.Pressed)
                commandInput.Add(DefaultKeys.AltShoot);

            if (input.MiddleButton == ButtonState.Pressed)
                commandInput.Add(DefaultKeys.Boost);

            var deltaScrollWheelValue = 0;

            var nextScrollValue = input.ScrollWheelValue;

            deltaScrollWheelValue = nextScrollValue - currentScrollWheelValue;
            currentScrollWheelValue += deltaScrollWheelValue;

            if (deltaScrollWheelValue != 0)
            {
                var factor = currentScrollWheelValue / 120;

                var nextZoom = (1 + ((float)factor / 10f));

                if (nextZoom > 0.5 && nextZoom < 1.5)
                {
                    zoom = nextZoom;
                    commandInput.Add(DefaultKeys.ZoomIn);
                }
                else
                {
                    commandInput.Add(DefaultKeys.ZoomOut);
                }
            }
            return commandInput;
        }
    }
}